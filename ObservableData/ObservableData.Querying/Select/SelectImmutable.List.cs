using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using ObservableData.Querying.Utils.Adapters;

namespace ObservableData.Querying.Select
{
    internal static partial class SelectImmutable
    {
        public sealed class ListChangesObserver<T, TAdaptee> :
            ObserverAdapter<IBatch<IndexedChange<T>>, IBatch<IndexedChange<TAdaptee>>>
        {
            [NotNull] readonly SelectState<T, TAdaptee> _state = new SelectState<T, TAdaptee>();
            [NotNull] private readonly Func<T, TAdaptee> _func;

            public ListChangesObserver(
                [NotNull] IObserver<IBatch<IndexedChange<TAdaptee>>> adaptee,
                [NotNull] Func<T, TAdaptee> func)
                : base(adaptee)
            {
                _func = func;
            }

            public override void OnNext(IBatch<IndexedChange<T>> value)
            {
                if (value == null) return;

                this.Adaptee.OnNext(_state.Apply(value, _func));
            }
        }
        [NotNull]
        private static IBatch<IndexedChange<TAdaptee>> Apply<T, TAdaptee>(
            [NotNull] this SelectState<T, TAdaptee> state,
            [NotNull] IBatch<IndexedChange<T>> value,
            [NotNull] Func<T, TAdaptee> func)
        {
            Dictionary<T, TAdaptee> removedOnChange = null;

            foreach (var update in value.GetIterations())
            {
                switch (update.Type)
                {
                    case IndexedChangeType.Add:
                        state.OnAdd(update.Item, func, removedOnChange);
                        break;

                    case IndexedChangeType.Remove:
                        state.OnRemove(update.Item, ref removedOnChange);
                        break;

                    case IndexedChangeType.Move:
                        break;

                    case IndexedChangeType.Replace:
                        state.OnRemove(update.ChangedItem, ref removedOnChange);
                        state.OnAdd(update.Item, func, removedOnChange);
                        break;

                    case IndexedChangeType.Clear:
                        state.Clear();
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            var adapter = new ListChanges<T, TAdaptee>(value, state, removedOnChange);
            return adapter;
        }

        public sealed class ListDataObserver<T, TAdaptee> :
            ObserverAdapter<IndexedChangesPlusState<T>, IndexedChangesPlusState<TAdaptee>>
        {
            [NotNull] readonly SelectState<T, TAdaptee> _state = new SelectState<T, TAdaptee>();
            [NotNull] private readonly Func<T, TAdaptee> _func;

            public ListDataObserver(
                [NotNull] IObserver<IndexedChangesPlusState<TAdaptee>> adaptee,
                [NotNull] Func<T, TAdaptee> func)
                : base(adaptee)
            {
                _func = func;
            }

            public override void OnNext(IndexedChangesPlusState<T> value)
            {
                var change = _state.Apply(value.Changes, _func);
                var list = new StateAdapter(value.ReachedState, _state);
                this.Adaptee.OnNext(new IndexedChangesPlusState<TAdaptee>(change, list));
            }

            private sealed class StateAdapter : IReadOnlyList<TAdaptee>
            {
                [NotNull] private readonly IReadOnlyList<T> _adaptee;
                [NotNull] private readonly SelectState<T, TAdaptee> _state;

                public StateAdapter(
                    [NotNull] IReadOnlyList<T> adaptee,
                    [NotNull] SelectState<T, TAdaptee> state)
                {
                    _adaptee = adaptee;
                    _state = state;
                }

                public IEnumerator<TAdaptee> GetEnumerator()
                {
                    return _adaptee.Select(item => _state[item]).GetEnumerator();
                }

                IEnumerator IEnumerable.GetEnumerator()
                {
                    return this.GetEnumerator();
                }

                public int Count => _adaptee.Count;

                public TAdaptee this[int index] => _state[_adaptee[index]];
            }
        }

        private sealed class ListChanges<T, TAdaptee> : ThreadSensitiveChange<IndexedChange<TAdaptee>>
        {
            [NotNull] private readonly IBatch<IndexedChange<T>> _adaptee;
            [NotNull] private readonly SelectState<T, TAdaptee> _state;
            [CanBeNull] private readonly Dictionary<T, TAdaptee> _removed;

            public ListChanges(
                [NotNull] IBatch<IndexedChange<T>> adaptee,
                [NotNull] SelectState<T, TAdaptee> state,
                [CanBeNull] Dictionary<T, TAdaptee> removed)
            {
                _adaptee = adaptee;
                _state = state;
                _removed = removed;
            }

            protected override IEnumerable<IndexedChange<TAdaptee>> Enumerate()
            {
                foreach (var update in _adaptee.GetIterations())
                {
                    switch (update.Type)
                    {
                        case IndexedChangeType.Add:
                            yield return IndexedChange<TAdaptee>.OnAdd(
                                _state.Get(update.Item, _removed),
                                update.Index);
                            break;

                        case IndexedChangeType.Remove:
                            yield return IndexedChange<TAdaptee>.OnRemove(
                                _state.Get(update.Item, _removed),
                                update.Index);
                            break;

                        case IndexedChangeType.Move:
                            yield return IndexedChange<TAdaptee>.OnMove(
                                _state.Get(update.Item, _removed),
                                update.Index,
                                update.OriginalIndex);
                            break;

                        case IndexedChangeType.Replace:
                            yield return IndexedChange<TAdaptee>.OnReplace(
                                _state.Get(update.Item, _removed),
                                _state.Get(update.ChangedItem, _removed),
                                update.Index);
                            break;

                        case IndexedChangeType.Clear:
                            yield return IndexedChange<TAdaptee>.OnClear();
                            break;

                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
        }
    }
}