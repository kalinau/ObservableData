using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using ObservableData.Querying.Utils;

namespace ObservableData.Querying.Subsets
{
    internal class Take
    {
        public sealed class Observer<T> : IObserver<IIndexedNextValue<T>>
        {
            private StateAdapter<T> _stateAdapter = null;
            private List<T> _rechedState;

            [NotNull] private readonly IObserver<IIndexedNextValue<T>> _adaptee;
            private readonly int _limit;

            public Observer(
                [NotNull] IObserver<IIndexedNextValue<T>> adaptee, 
                int limit)
            {
                _adaptee = adaptee;
                _limit = limit;
                _rechedState = new List<T>(_limit);
            }

            public void OnNext(IIndexedNextValue<T> value)
            {
                value?.Match(this.OnState, this.OnChange);
            }

            private void OnState(IReadOnlyList<T> state)
            {
                if (state == null) return;

                _stateAdapter = new StateAdapter<T>(state, _limit);
                _rechedState = null;

                _adaptee.OnNext(_stateAdapter);
            }

            private void OnChange(IBatch<IndexedChange<T>> change)
            {
                if (change == null) return;

                if (_stateAdapter == null)
                {
                    _rechedState = new List<T>();
                    _stateAdapter = new StateAdapter<T>(_rechedState, _limit);
                }
                _rechedState?.Apply(change);
                _adaptee.OnNext(new ChangesAdapter<T>(change, _stateAdapter));
            }

            public void OnCompleted() => _adaptee.OnCompleted();

            public void OnError(Exception error) => _adaptee.OnError(error);
        }

        private sealed class ChangesAdapter<TItem> : 
            IBatch<GeneralChange<TItem>>, 
            IBatch<IndexedChange<TItem>>,
            IIndexedNextValue<TItem>
        {
            [NotNull] private readonly IBatch<IndexedChange<TItem>> _adaptee;
            [NotNull] private readonly StateAdapter<TItem> _state;

            public ChangesAdapter(
                [NotNull] IBatch<IndexedChange<TItem>> adaptee, 
                [NotNull] StateAdapter<TItem> state)
            {
                _adaptee = adaptee;
                _state = state;
            }

            public IEnumerable<GeneralChange<TItem>> GetPeaces()
            {
                foreach (var update in _adaptee.GetPeaces())
                {
                    if (update.Type == GeneralChangeType.Clear)
                    {
                        yield return update;
                    }
                    else if (_criterion.Invoke(update.Item))
                    {
                        yield return update;
                    }
                }
            }

            IEnumerable<IndexedChange<TItem>> IBatch<IndexedChange<TItem>>.GetPeaces()
            {
                foreach (var update in _adaptee.GetPeaces())
                {
                    switch (update.Type)
                    {
                        case IndexedChangeType.Add:
                            break;
                        case IndexedChangeType.Remove:
                            break;
                        case IndexedChangeType.Move:
                            break;
                        case IndexedChangeType.Replace:
                            if (update.Index < _state.Limit)
                            {
                                
                            }
                            break;

                        case IndexedChangeType.Clear:
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }

            public void MakeImmutable() => _adaptee.MakeImmutable();

            public void Match(Action<IReadOnlyList<TItem>> onState, Action<IBatch<IndexedChange<TItem>>> onChange)
            {
                onChange?.Invoke(this);
            }

            public T Match<T>(Func<IReadOnlyList<TItem>, T> onState, Func<IBatch<IndexedChange<TItem>>, T> onChange)
            {
                return onChange.Invoke(this);
            }

            public void Match(Action<IReadOnlyCollection<TItem>> onState, Action<IBatch<GeneralChange<TItem>>> onChange)
            {
                onChange?.Invoke(this);
            }

            public T Match<T>(Func<IReadOnlyCollection<TItem>, T> onState, Func<IBatch<GeneralChange<TItem>>, T> onChange)
            {
                return onChange.Invoke(this);
            }
        }

        private sealed class StateAdapter<TItem> : State<TItem>,IReadOnlyList<TItem>
        {
            [NotNull] private readonly IReadOnlyList<TItem> _source;
            private readonly int _limit;

            public StateAdapter(
                [NotNull] IReadOnlyList<TItem> source,
                int limit)
            {
                _source = source;
                _limit = limit;
            }

            protected override IReadOnlyList<TItem> GetList => this;

            public int Count => System.Math.Min(_limit, _source.Count);

            public int Limit => _limit;

            public TItem this[int index]
            {
                get
                {
                    Index.Check(index, _limit);
                    return _source[index];
                }
            }

            public IEnumerator<TItem> GetEnumerator()
            {
                return _source.Take(_limit).GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
        }

        private abstract class State<TItem> : IIndexedNextValue<TItem>
        {
            protected abstract IReadOnlyList<TItem> GetList { get; }

            public void Match(Action<IReadOnlyList<TItem>> onState, Action<IBatch<IndexedChange<TItem>>> onChange)
            {
                onState?.Invoke(this.GetList);
            }

            public T Match<T>(Func<IReadOnlyList<TItem>, T> onState, Func<IBatch<IndexedChange<TItem>>, T> onChange)
            {
                return onState(this.GetList);
            }

            public void Match(Action<IReadOnlyCollection<TItem>> onState, Action<IBatch<GeneralChange<TItem>>> onChange)
            {
                onState?.Invoke(this.GetList);
            }

            public T Match<T>(Func<IReadOnlyCollection<TItem>, T> onState, Func<IBatch<GeneralChange<TItem>>, T> onChange)
            {
                return onState(this.GetList);
            }
        }
    }
}
