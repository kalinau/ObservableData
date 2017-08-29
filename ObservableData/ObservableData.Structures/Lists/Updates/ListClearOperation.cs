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

        IEnumerable<IndexedChange<T>> IBatch<IndexedChange<T>>.GetPeaces()
        {
            yield return IndexedChange<T>.OnClear();
        }

        IEnumerable<GeneralChange<T>> IBatch<GeneralChange<T>>.GetPeaces()
        {
            yield return GeneralChange<T>.OnClear();
        }

        IEnumerable<IListOperation<T>> IBatch<IListOperation<T>>.GetPeaces()
        {
            yield return this;
        }

        IEnumerable<ICollectionOperation<T>> IBatch<ICollectionOperation<T>>.GetPeaces()
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