using System;
using System.Collections.Generic;

namespace ObservableData.Structures.Lists.Updates
{
    public abstract class ListBaseOperation<T> : IListOperation<T>, IListChange<T>
    {
        public ListBaseOperation<T> Next { get; set; }

        public abstract void Lock();

        public abstract ICollectionOperation<T> TryGetCollectionOperation();

        public abstract TResult Match<TResult>(
            Func<IListInsertOperation<T>, TResult> onInsert,
            Func<IListRemoveOperation<T>, TResult> onRemove, 
            Func<IListReplaceOperation<T>, TResult> onReplace,
            Func<IListMoveOperation<T>, TResult> onMove, 
            Func<IListResetOperation<T>, TResult> onReset);

        public abstract void Match(
            Action<IListInsertOperation<T>> onInsert,
            Action<IListRemoveOperation<T>> onRemove,
            Action<IListReplaceOperation<T>> onReplace,
            Action<IListMoveOperation<T>> onMove,
            Action<IListResetOperation<T>> onReset);

        IEnumerable<IListOperation<T>> IChange<IListOperation<T>>.Operations()
        {
            yield return this;
        }

        IEnumerable<ICollectionOperation<T>> IChange<ICollectionOperation<T>>.Operations()
        {
            var operation = this.TryGetCollectionOperation();
            if (operation != null)
            {
                yield return operation;
            }
        }
    }
}