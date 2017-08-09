using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using ObservableData.Structures;

namespace ObservableData.Querying.Utils
{
    //TODO: verify
    [PublicAPI]
    public static class ListOperationExtensions
    {
        private sealed class ListChangeAdapter<T> : IChange<CollectionOperation<T>>
        {
            [NotNull] private readonly IChange<ListOperation<T>> _adaptee;

            public ListChangeAdapter([NotNull] IChange<ListOperation<T>> adaptee)
            {
                _adaptee = adaptee;
            }

            public IEnumerable<CollectionOperation<T>> Operations() => _adaptee.Operations().AsCollectionOperations();

            public void Lock() => _adaptee.Lock();
        }


        [NotNull]
        public static IChange<CollectionOperation<T>> AsCollectionUpdate<T>([NotNull] this IChange<ListOperation<T>> change) =>
            new ListChangeAdapter<T>(change);

        [NotNull]
        public static IEnumerable<CollectionOperation<T>> AsCollectionOperations<T>([NotNull] this IEnumerable<ListOperation<T>> operations)
        {
            foreach (var update in operations)
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
    }
}
