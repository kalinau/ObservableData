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
        private class AddItemsEnumerator<TIn, TOut> : ICollectionChangeEnumerator<TIn>
        {
            [NotNull] private readonly Map<TIn, TOut> _map;
            [NotNull] private readonly Func<TIn, TOut> _selector;

            private bool _isStoped = true;
            private int _clearCount;
            private int _removeCount;
            private int _addCount;

            public AddItemsEnumerator(
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

        private class RemoveItemsEnumerator<TIn, TOut> : ICollectionChangeEnumerator<TIn>
        {
            [CanBeNull] private Map<TIn, TOut> _startState = new Map<TIn, TOut>();

            [NotNull] private readonly Map<TIn, TOut> _map;
            private int _clearCount;

            public RemoveItemsEnumerator([NotNull] Map<TIn, TOut> map)
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

        public sealed class CollectionObserver<TIn, TOut> :
            IObserver<ICollectionChange<TIn>>,
            ICollectionChangeEnumerator<TIn>,
            ICollectionChange<TOut>
        {
            private sealed class State : IReadOnlyCollection<TOut>
            {
                [NotNull] private IReadOnlyCollection<TIn> _source = EmptyList<TIn>.Instance;
                [NotNull] private readonly Map<TIn, TOut> _map;

                public State([NotNull] Map<TIn, TOut> map)
                {
                    _map = map;
                }

                public void ChangeSource([NotNull] IReadOnlyCollection<TIn> source)
                {
                    _source = source;
                }

                public int Count => _source.Count;

                public IEnumerator<TOut> GetEnumerator() => 
                    _source.Select(item => _map[item]).GetEnumerator();

                IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
            }

            [NotNull] private readonly IObserver<ICollectionChange<TOut>> _adaptee;
            [NotNull] private readonly Map<TIn, TOut> _map;
            [NotNull] private readonly AddItemsEnumerator<TIn, TOut> _adder;
            [NotNull] private readonly RemoveItemsEnumerator<TIn, TOut> _remover;
            [NotNull] private readonly State _state;

            private ThreadId? _thread;
            private ICollectionChange<TIn> _change;
            private ICollectionChangeEnumerator<TOut> _enumerator;

            public CollectionObserver(
                [NotNull] IObserver<ICollectionChange<TOut>> adaptee,
                [NotNull] Func<TIn, TOut> selector)
            {
                _adaptee = adaptee;
                _map = new Map<TIn, TOut>();
                _adder = new AddItemsEnumerator<TIn, TOut>(_map, selector);
                _remover = new RemoveItemsEnumerator<TIn, TOut>(_map);
                _state = new State(_map);
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
                _state.ChangeSource(state);
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

        public sealed class ListObserver<TIn, TOut> :
            IObserver<IListChange<TIn>>,
            IListChangeEnumerator<TIn>,
            IListChange<TOut>
        {
            private sealed class State : IReadOnlyList<TOut>
            {
                [NotNull] private IReadOnlyList<TIn> _source;
                [NotNull] private readonly Map<TIn, TOut> _map;

                public State(
                    [NotNull] IReadOnlyList<TIn> source,
                    [NotNull] Map<TIn, TOut> map)
                {
                    _source = source;
                    _map = map;
                }

                public void ChangeSource([NotNull] IReadOnlyList<TIn> source)
                {
                    _source = source;
                }

                public int Count => _source.Count;

                public IEnumerator<TOut> GetEnumerator() =>
                    _source.Select(item => _map[item]).GetEnumerator();

                IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

                public TOut this[int index] => _map[_source[index]];
            }

            [NotNull] private readonly IObserver<IListChange<TOut>> _adaptee;
            [NotNull] private readonly Map<TIn, TOut> _map;
            [NotNull] private readonly AddItemsEnumerator<TIn, TOut> _adder;
            [NotNull] private readonly RemoveItemsEnumerator<TIn, TOut> _remover;
            
            [CanBeNull] private State _state;

            private ThreadId? _thread;
            private IListChange<TIn> _change;
            private IListChangeEnumerator<TOut> _enumerator;

            [CanBeNull] private ListToCollectionChangeEnumerator<TOut> _collectionEnumeratorBuffer;

            public ListObserver(
                [NotNull] IObserver<IListChange<TOut>> adaptee,
                [NotNull] Func<TIn, TOut> selector)
            {
                _adaptee = adaptee;
                _map = new Map<TIn, TOut>();
                _adder = new AddItemsEnumerator<TIn, TOut>(_map, selector);
                _remover = new RemoveItemsEnumerator<TIn, TOut>(_map);
            }

            void IObserver<IListChange<TIn>>.OnCompleted() => _adaptee.OnCompleted();

            void IObserver<IListChange<TIn>>.OnError(Exception error) => _adaptee.OnError(error);

            void IObserver<IListChange<TIn>>.OnNext(IListChange<TIn> value)
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
                _enumerator = enumerator.FromBuffer(ref _collectionEnumeratorBuffer);
                change.Enumerate(this);
                _adder.Stop();
            }

            void IListChange<TOut>.Enumerate(IListChangeEnumerator<TOut> enumerator)
            {
                var change = _change.Check(_thread);
                _enumerator = enumerator;
                change.Enumerate(this);
                _adder.Stop();
            }

            void IListChangeEnumerator<TIn>.OnStateChanged(IReadOnlyList<TIn> state)
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

            void IListChangeEnumerator<TIn>.OnClear()
            {
                var e = _enumerator.Check(_thread);
                _adder.OnClear();
                e.OnClear();
            }

            void IListChangeEnumerator<TIn>.OnAdd(TIn item, int index)
            {
                var e = _enumerator.Check(_thread);
                _adder.OnAdd(item, index);
                e.OnAdd(_map[item], index);
            }

            void IListChangeEnumerator<TIn>.OnRemove(TIn item, int index)
            {
                var e = _enumerator.Check(_thread);
                _adder.OnRemove(item, index);
                e.OnRemove(_map[item], index);
            }

            void IListChangeEnumerator<TIn>.OnMove(TIn item, int index, int originalIndex)
            {
                var e = _enumerator.Check(_thread);
                _adder.OnMove(item, index, originalIndex);
                e.OnMove(_map[item], index, originalIndex);
            }

            void IListChangeEnumerator<TIn>.OnReplace(TIn item, TIn changedItem, int index)
            {
                var e = _enumerator.Check(_thread);
                _adder.OnReplace(item, changedItem, index);
                e.OnReplace(_map[item], _map[changedItem], index);
            }
        }
    }
}