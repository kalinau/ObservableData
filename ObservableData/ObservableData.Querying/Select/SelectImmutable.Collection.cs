using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using ObservableData.Querying.Utils.Adapters;

namespace ObservableData.Querying.Select
{
    internal static partial class SelectImmutable
    {
        public sealed class CollectionChangesObserver<T, TAdaptee> : ObserverAdapter<IChange<CollectionOperation<T>>,
            ChangedCollectionData<TAdaptee>>
        {
            [NotNull] readonly SelectState<T, TAdaptee> _state = new SelectState<T, TAdaptee>();
            [NotNull] private readonly Func<T, TAdaptee> _func;

            public CollectionChangesObserver(
                [NotNull] IObserver<ChangedCollectionData<TAdaptee>> adaptee,
                [NotNull] Func<T, TAdaptee> func)
                : base(adaptee)
            {
                _func = func;
            }

            public override void OnNext(IChange<CollectionOperation<T>> value)
            {
                if (value == null) return;

                Dictionary<T, TAdaptee> removedOnChange = null;

                foreach (var update in value.GetIterations())
                {
                    switch (update.Type)
                    {
                        case CollectionOperationType.Add:
                            _state.OnAdd(update.Item, _func, removedOnChange);
                            break;

                        case CollectionOperationType.Remove:
                            _state.OnRemove(update.Item, ref removedOnChange);
                            break;

                        case CollectionOperationType.Clear:
                            _state.Clear();
                            break;

                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                var adapter = new Change(value, _state, removedOnChange);
                var result = new ChangedCollectionData<TAdaptee>(adapter, _state);
                this.Adaptee.OnNext(result);
            }

            private sealed class Change : ChangeWithLock<CollectionOperation<TAdaptee>>
            {
                [NotNull] private readonly IChange<CollectionOperation<T>> _adaptee;
                [NotNull] private readonly SelectState<T, TAdaptee> _state;
                [CanBeNull] private readonly Dictionary<T, TAdaptee> _removedOnChange;

                public Change(
                    [NotNull] IChange<CollectionOperation<T>> adaptee,
                    [NotNull] SelectState<T, TAdaptee> state,
                    [CanBeNull] Dictionary<T, TAdaptee> removedOnChange)
                {
                    _adaptee = adaptee;
                    _state = state;
                    _removedOnChange = removedOnChange;
                }

                protected override IEnumerable<CollectionOperation<TAdaptee>> Enumerate()
                {
                    foreach (var update in _adaptee.GetIterations())
                    {
                        switch (update.Type)
                        {
                            case CollectionOperationType.Add:
                                var added = _state.Get(update.Item, _removedOnChange);
                                yield return CollectionOperation<TAdaptee>.OnAdd(added);
                                break;

                            case CollectionOperationType.Remove:
                                var removed = _state.Get(update.Item, _removedOnChange);
                                yield return CollectionOperation<TAdaptee>.OnRemove(removed);
                                break;

                            case CollectionOperationType.Clear:
                                yield return CollectionOperation<TAdaptee>.OnClear();
                                break;

                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                }
            }
        }
    }
}