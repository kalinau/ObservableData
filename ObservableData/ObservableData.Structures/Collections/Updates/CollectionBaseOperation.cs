using System;
using System.Collections.Generic;

namespace ObservableData.Structures.Collections.Updates
{
    public abstract class CollectionBaseOperation<T> : ICollectionOperation<T>, IChange<ICollectionOperation<T>>
    {
        public abstract void Lock();

        IEnumerable<ICollectionOperation<T>> IChange<ICollectionOperation<T>>.Operations()
        {
            yield return this;
        }

        public abstract TResult Match<TResult>(
            Func<ICollectionInsertOperation<T>, TResult> onInsert,
            Func<ICollectionRemoveOperation<T>, TResult> onRemove,
            Func<ICollectionReplaceOperation<T>, TResult> onReplace,
            Func<ICollectionResetOperation<T>, TResult> onReset);

        public abstract void Match(
            Action<ICollectionInsertOperation<T>> onInsert,
            Action<ICollectionRemoveOperation<T>> onRemove,
            Action<ICollectionReplaceOperation<T>> onReplace,
            Action<ICollectionResetOperation<T>> onReset);
    }
}