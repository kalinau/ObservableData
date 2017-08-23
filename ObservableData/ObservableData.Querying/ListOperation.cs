using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace ObservableData.Querying
{
    public struct ListOperation<T>
    {
        private ListOperation(ListOperationType type, T item, T changedItem, int index, int originalIndex)
        {
            this.Type = type;
            this.Item = item;
            this.ChangedItem = changedItem;
            this.Index = index;
            this.OriginalIndex = originalIndex;
        }

        public static ListOperation<T> OnClear()
        {
            return new ListOperation<T>(ListOperationType.Clear, default(T), default(T), default(int), default(int));
        }

        public static ListOperation<T> OnAdd(T item, int index)
        {
            return new ListOperation<T>(ListOperationType.Add, item, default(T), index, default(int));
        }

        public static ListOperation<T> OnRemove(T item, int index)
        {
            return new ListOperation<T>(ListOperationType.Remove, item, default(T), index, default(int));
        }

        public static ListOperation<T> OnMove(T item, int index, int originalIndex)
        {
            return new ListOperation<T>(ListOperationType.Move, item, default(T), index, originalIndex);
        }

        public static ListOperation<T> OnReplace(T item, T changedItem, int index)
        {
            return new ListOperation<T>(ListOperationType.Replace, item, changedItem, index, default(int));
        }

        public ListOperationType Type { get; }

        [CanBeNull]
        public T Item { get; }

        [CanBeNull]
        public T ChangedItem { get; }

        public int Index { get; }

        public int OriginalIndex { get; }
    }

    public static class ListOperationExtensions
    {
        [NotNull]
        public static IEnumerable<CollectionOperation<T>> AsForCollection<T>(this ListOperation<T> operation)
        {
            switch (operation.Type)
            {
                case ListOperationType.Add:
                    yield return CollectionOperation<T>.OnAdd(operation.Item);
                    break;

                case ListOperationType.Remove:
                    yield return CollectionOperation<T>.OnRemove(operation.Item);
                    break;

                case ListOperationType.Move:
                    break;

                case ListOperationType.Replace:
                    yield return CollectionOperation<T>.OnRemove(operation.ChangedItem);
                    yield return CollectionOperation<T>.OnAdd(operation.Item);
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