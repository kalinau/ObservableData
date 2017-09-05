using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using ObservableData.Querying.Utils;

namespace ObservableData.Querying.Select
{
    internal static partial class SelectImmutable
    {
        public sealed class CollectionObserver<TIn, TOut> :
            IObserver<ICollectionChange<TIn>>,
            ICollectionChangeEnumerator<TIn>,
            ICollectionChange<TOut>
        {
            private class FirstEnumerator : ICollectionChangeEnumerator<TIn>
            {
                [NotNull] private readonly Map<TIn, TOut> _map;
                [NotNull] private readonly Func<TIn, TOut> _selector;

                private bool _isStoped = true;
                private int _clearCount;
                private int _removeCount;
                private int _addCount;

                public FirstEnumerator(
                    [NotNull] Map<TIn, TOut> map, 
                    [NotNull] Func<TIn, TOut> selector)
                {
                    _map = map;
                    _selector = selector;
                }

                public bool IsStoped => _isStoped;

                public int ClearCount => _clearCount;

                public int RemoveCount => _removeCount;

                public int AddCount => _addCount;

                public void Restart()
                {
                    _isStoped = false;
                    _clearCount = 0;
                    _removeCount = 0;
                    _addCount = 0;
                }

                public void Stop()
                {
                    _isStoped = true;
                }

                public void OnStateChanged(IReadOnlyCollection<TIn> state)
                {
                    _map.Clear();
                    foreach (var item in state)
                    {
                        if (!_map.TryIncreaseCount(item))
                        {
                            _map.Add(item, _selector(item));
                        }
                    }
                    _clearCount = 0;
                    _removeCount = 0;
                    _addCount = 0;
                    _isStoped = true;
                }

                public void OnClear()
                {
                    if (_isStoped) return;
                    if (_map.IsEmpty) return;

                    _clearCount++;
                    _removeCount = 0;
                    _addCount = 0;
                }

                public void OnAdd(TIn item)
                {
                    if (_isStoped) return;
                    if (!_map.TryIncreaseCount(item))
                    {
                        _map.Add(item, _selector(item));
                    }
                    _addCount++;
                }

                public void OnRemove(TIn item)
                {
                    if (_isStoped) return;
                    _removeCount++;
                }
            }

            private class SecondEnumerator : ICollectionChangeEnumerator<TIn>
            {
                [CanBeNull] private Map<TIn, TOut> _startState = new Map<TIn, TOut>();

                [NotNull] private readonly Map<TIn, TOut> _map;
                private int _clearCount;

                public SecondEnumerator([NotNull] Map<TIn, TOut> map)
                {
                    _map = map;
                }

                public bool TryRestart(int add, int remove, int clear)
                {
                    _clearCount = clear;

                    _startState = null;
                    if (clear > 0)
                    {
                        if (add == 0)
                        {
                            _map.Clear();
                        }
                        else
                        {
                            _startState = _map.CloneAndClear();
                        }
                    }
                    return clear > 0 || remove > 0;
                }

                public void OnStateChanged(IReadOnlyCollection<TIn> state)
                {
                    throw new InvalidOperationException();
                }

                public void OnClear()
                {
                    _clearCount--;
                }

                public void OnAdd(TIn item)
                {
                    if (_clearCount == 0 && _startState != null)
                    {
                        if (!_map.TryIncreaseCount(item))
                        {
                            _map.Add(item, _startState[item]);
                        }
                    }
                }

                public void OnRemove(TIn item)
                {
                    if (_clearCount == 0)
                    {
                        _map.DecreaseCount(item);
                    }
                }
            }

            private sealed class State : IReadOnlyCollection<TOut>
            {
                [NotNull] private IReadOnlyCollection<TIn> _source;
                [NotNull] private readonly Map<TIn, TOut> _state;

                public State(
                    [NotNull] IReadOnlyCollection<TIn> source,
                    [NotNull] Map<TIn, TOut> state)
                {
                    _source = source;
                    _state = state;
                }

                public void ChangeSource([NotNull] IReadOnlyCollection<TIn> source)
                {
                    _source = source;
                }

                public int Count => _source.Count;

                public IEnumerator<TOut> GetEnumerator() => 
                    _source.Select(item => _state[item]).GetEnumerator();

                IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
            }

            [NotNull] private readonly IObserver<ICollectionChange<TOut>> _adaptee;
            [NotNull] private readonly Map<TIn, TOut> _map;
            [NotNull] private readonly FirstEnumerator _adder;
            [NotNull] private readonly SecondEnumerator _remover;

            [CanBeNull] private State _state;

            private ThreadId? _thread;
            private ICollectionChange<TIn> _change;
            private ICollectionChangeEnumerator<TOut> _enumerator;

            public CollectionObserver(
                [NotNull] IObserver<ICollectionChange<TOut>> adaptee,
                [NotNull] Func<TIn, TOut> selector)
            {
                _adaptee = adaptee;
                _map = new Map<TIn, TOut>();
                _adder = new FirstEnumerator(_map, selector);
                _remover = new SecondEnumerator(_map);
            }

            void IObserver<ICollectionChange<TIn>>.OnCompleted() => _adaptee.OnCompleted();

            void IObserver<ICollectionChange<TIn>>.OnError(Exception error) => _adaptee.OnError(error);

            void IObserver<ICollectionChange<TIn>>.OnNext(ICollectionChange<TIn> value)
            {
                if (value == null) return;

                _thread = ThreadId.FromCurrent();

                _change = value;

                _adder.Restart();
                _adaptee.OnNext(this);

                if (_adder.IsStoped == false)
                {
                    _change.Enumerate(_adder);
                }
                if (_remover.TryRestart(_adder.AddCount, _adder.RemoveCount, _adder.ClearCount))
                {
                    _change.Enumerate(_remover);
                }
                _thread = null;
            }

            void ICollectionChange<TOut>.Enumerate(ICollectionChangeEnumerator<TOut> enumerator)
            {
                var change = _change.Check(_thread);
                _enumerator = enumerator;
                change.Enumerate(this);
                _adder.Stop();
            }

            void ICollectionChangeEnumerator<TIn>.OnStateChanged(IReadOnlyCollection<TIn> state)
            {
                var e = _enumerator.Check(_thread);
                _adder.OnStateChanged(state);
                if (_state == null)
                {
                    _state = new State(state, _map);
                }
                else
                {
                    _state.ChangeSource(state);
                }
                e.OnStateChanged(_state);
                _enumerator = null;
            }

            void ICollectionChangeEnumerator<TIn>.OnClear()
            {
                var e = _enumerator.Check(_thread);
                _adder.OnClear();
                e.OnClear();
            }

            void ICollectionChangeEnumerator<TIn>.OnAdd(TIn item)
            {
                var e = _enumerator.Check(_thread);
                _adder.OnAdd(item);
                e.OnAdd(_map[item]);
            }

            void ICollectionChangeEnumerator<TIn>.OnRemove(TIn item)
            {
                var e = _enumerator.Check(_thread);
                _adder.OnRemove(item);
                e.OnRemove(_map[item]);
            }
        }
    }
}