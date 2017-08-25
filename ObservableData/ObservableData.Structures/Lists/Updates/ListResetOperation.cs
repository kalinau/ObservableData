using System;
using System.Collections.Generic;
using ObservableData.Querying;

namespace ObservableData.Structures.Lists.Updates
{
    public class ListClearOperation<T> :
        IListChangeNode<T>,
        IListClearOperation<T>, 
        ICollectionClearOperation<T>
    {
        public static ListClearOperation<T> Instance { get; } = new ListClearOperation<T>();

        private ListClearOperation()
        {
        }

        IListChangeNode<T> IListChangeNode<T>.Next { get; set; }

        public void MakeImmutable() { }

        IEnumerable<ListOperation<T>> IChange<ListOperation<T>>.GetIterations()
        {
            yield return ListOperation<T>.OnClear();
        }

        IEnumerable<CollectionOperation<T>> IChange<CollectionOperation<T>>.GetIterations()
        {
            yield return CollectionOperation<T>.OnClear();
        }

        IEnumerable<IListOperation<T>> IChange<IListOperation<T>>.GetIterations()
        {
            yield return this;
        }

        IEnumerable<ICollectionOperation<T>> IChange<ICollectionOperation<T>>.GetIterations()
        {
            yield return this;
        }

        TResult IListOperation<T>.Match<TResult>(Func<IListInsertOperation<T>, TResult> onInsert, Func<IListRemoveOperation<T>, TResult> onRemove, Func<IListReplaceOperation<T>, TResult> onReplace, Func<IListMoveOperation<T>, TResult> onMove, Func<IListClearOperation<T>, TResult> onReset)
        {
            return onReset.Invoke(this);
        }

        void IListOperation<T>.Match(Action<IListInsertOperation<T>> onInsert, Action<IListRemoveOperation<T>> onRemove, Action<IListReplaceOperation<T>> onReplace, Action<IListMoveOperation<T>> onMove, Action<IListClearOperation<T>> onReset)
        {
            onReset?.Invoke(this);
        }

        TResult ICollectionOperation<T>.Match<TResult>(Func<ICollectionInsertOperation<T>, TResult> onInsert, Func<ICollectionRemoveOperation<T>, TResult> onRemove, Func<ICollectionReplaceOperation<T>, TResult> onReplace, Func<ICollectionClearOperation<T>, TResult> onReset)
        {
            return onReset.Invoke(this);
        }

        void ICollectionOperation<T>.Match(Action<ICollectionInsertOperation<T>> onInsert, Action<ICollectionRemoveOperation<T>> onRemove, Action<ICollectionReplaceOperation<T>> onReplace, Action<ICollectionClearOperation<T>> onReset)
        {
            onReset?.Invoke(this);
        }
    }
}