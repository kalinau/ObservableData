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
            ObserverAdapter<IChange<ListOperation<T>>, IChange<ListOperation<TAdaptee>>>
        {
            [NotNull] readonly SelectState<T, TAdaptee> _state = new SelectState<T, TAdaptee>();
            [NotNull] private readonly Func<T, TAdaptee> _func;

            public ListChangesObserver(
                [NotNull] IObserver<IChange<ListOperation<TAdaptee>>> adaptee,
                [NotNull] Func<T, TAdaptee> func)
                : base(adaptee)
            {
                _func = func;
            }

            public override void OnNext(IChange<ListOperation<T>> value)
            {
                if (value == null) return;

                this.Adaptee.OnNext(_state.Apply(value, _func));
            }
        }

        public sealed class ListDataObserver<T, TAdaptee> :
            ObserverAdapter<ChangedListData<T>, ChangedListData<TAdaptee>>
        {
            [NotNull] readonly SelectState<T, TAdaptee> _state = new SelectState<T, TAdaptee>();
            [NotNull] private readonly Func<T, TAdaptee> _func;

            public ListDataObserver(
                [NotNull] IObserver<ChangedListData<TAdaptee>> adaptee,
                [NotNull] Func<T, TAdaptee> func)
                : base(adaptee)
            {
                _func = func;
            }

            public override void OnNext(ChangedListData<T> value)
            {
                var change = _state.Apply(value.Change, _func);
                var list = new StateAdapter(value.ReachedState, _state);
                this.Adaptee.OnNext(new ChangedListData<TAdaptee>(change, list));
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

        [NotNull]
        private static IChange<ListOperation<TAdaptee>> Apply<T, TAdaptee>(
            [NotNull] this SelectState<T, TAdaptee> state,
            [NotNull] IChange<ListOperation<T>> value,
            [NotNull] Func<T, TAdaptee> func)
        {
            Dictionary<T, TAdaptee> removedOnChange = null;

            foreach (var update in value.Iterations())
            {
                switch (update.Type)
                {
                    case ListOperationType.Add:
                        state.OnAdd(update.Item, func, removedOnChange);
                        break;

                    case ListOperationType.Remove:
                        state.OnRemove(update.Item, ref removedOnChange);
                        break;

                    case ListOperationType.Move:
                        break;

                    case ListOperationType.Replace:
                        state.OnRemove(update.Item, ref removedOnChange);
                        state.OnAdd(update.Item, func, removedOnChange);
                        break;

                    case ListOperationType.Clear:
                        state.Clear();
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            var adapter = new ListChange<T, TAdaptee>(value, state, removedOnChange);
            return adapter;
        }

        private sealed class ListChange<T, TAdaptee> : ChangeWithLock<ListOperation<TAdaptee>>
        {
            [NotNull] private readonly IChange<ListOperation<T>> _adaptee;
            [NotNull] private readonly SelectState<T, TAdaptee> _state;
            [CanBeNull] private readonly Dictionary<T, TAdaptee> _removed;

            public ListChange(
                [NotNull] IChange<ListOperation<T>> adaptee,
                [NotNull] SelectState<T, TAdaptee> state,
                [CanBeNull] Dictionary<T, TAdaptee> removed)
            {
                _adaptee = adaptee;
                _state = state;
                _removed = removed;
            }

            protected override IEnumerable<ListOperation<TAdaptee>> Enumerate()
            {
                foreach (var update in _adaptee.Iterations())
                {
                    switch (update.Type)
                    {
                        case ListOperationType.Add:
                            yield return ListOperation<TAdaptee>.OnAdd(
                                _state.Get(update.Item, _removed),
                                update.Index);
                            break;

                        case ListOperationType.Remove:
                            yield return ListOperation<TAdaptee>.OnRemove(
                                _state.Get(update.Item, _removed),
                                update.Index);
                            break;

                        case ListOperationType.Move:
                            yield return ListOperation<TAdaptee>.OnMove(
                                _state.Get(update.Item, _removed),
                                update.Index,
                                update.OriginalIndex);
                            break;

                        case ListOperationType.Replace:
                            yield return ListOperation<TAdaptee>.OnReplace(
                                _state.Get(update.Item, _removed),
                                _state.Get(update.ChangedItem, _removed),
                                update.Index);
                            break;

                        case ListOperationType.Clear:
                            yield return ListOperation<TAdaptee>.OnClear();
                            break;

                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
        }
    }
}