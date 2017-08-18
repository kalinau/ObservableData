using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using JetBrains.Annotations;
using ObservableData.Querying.Utils;
using ObservableData.Structures;

namespace ObservableData.Querying
{
    public static partial class ObservableExtensions
    {
        private sealed class ListChangeAdapter<T> : IChange<CollectionOperation<T>>
        {
            [NotNull] private readonly IChange<ListOperation<T>> _adaptee;

            public ListChangeAdapter([NotNull] IChange<ListOperation<T>> adaptee)
            {
                _adaptee = adaptee;
            }

            public IEnumerable<CollectionOperation<T>> Operations()
            {
                foreach (var update in _adaptee.Operations())
                {
                    switch (update.Type)
                    {
                        case ListOperationType.Add:
                            yield return CollectionOperation<T>.OnAdd(update.Item);
                            break;

                        case ListOperationType.Remove:
                            yield return CollectionOperation<T>.OnRemove(update.Item);
                            break;

                        case ListOperationType.Move:
                            break;

                        case ListOperationType.Replace:
                            yield return CollectionOperation<T>.OnRemove(update.ChangedItem);
                            yield return CollectionOperation<T>.OnAdd(update.Item);
                            break;

                        case ListOperationType.Clear:
                            yield return CollectionOperation<T>.OnClear();
                            break;

                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }

            public void Lock()
            {
                _adaptee.Lock();
            }
        }

        [NotNull]
        public static IObservable<IChange<CollectionOperation<T>>> AsCollectionChanges<T>(
            [NotNull] this IObservable<IChange<ListOperation<T>>> previous)
        {
            return previous.Select(x => x == null ? null : new ListChangeAdapter<T>(x)).NotNull();
        }

        [NotNull]
        public static IObservable<ChangedCollectionData<T>> AsCollectionData<T>(
            [NotNull] this IObservable<ChangedListData<T>> previous)
        {
            return previous.Select(x => new ChangedCollectionData<T>(
                new ListChangeAdapter<T>(x.Change),
                x.ReachedState))
                .NotNull();
        }

        [NotNull]
        public static IObservable<IChange<ListOperation<T>>> AsChanges<T>(
            [NotNull] this IObservable<ChangedListData<T>> previous)
        {
            return previous.Select(x => x.Change).NotNull();
        }

        [NotNull]
        public static IObservable<IChange<CollectionOperation<T>>> AsChanges<T>(
            [NotNull] this IObservable<ChangedCollectionData<T>> previous)
        {
            return previous.Select(x => x.Change).NotNull();
        }
    }
}