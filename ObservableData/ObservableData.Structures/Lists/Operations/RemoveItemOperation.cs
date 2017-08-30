using System;
using System.Collections.Generic;
using ObservableData.Querying;
using ObservableData.Structures.Lists.Utils;

namespace ObservableData.Structures.Lists.Operations
{
    internal sealed class RemoveItemOperation<T> : 
        IListBatchChangeNode<T>,
        IListRemoveOperation<T>, 
        ICollectionRemoveOperation<T>
    {
        private readonly int _index;
        private readonly T _item;

        public RemoveItemOperation(int index, T item)
        {
            _index = index;
            _item = item;
        }

        IListBatchChangeNode<T> IListBatchChangeNode<T>.Next { get; set; }

        int IListRemoveOperation<T>.Index => _index;

        T IListRemoveOperation<T>.Item => _item;

        T ICollectionRemoveOperation<T>.Item => _item;

        TResult IListOperation<T>.Match<TResult>(
            Func<IListInsertOperation<T>, TResult> onInsert,
            Func<IListRemoveOperation<T>, TResult> onRemove,
            Func<IListReplaceOperation<T>, TResult> onReplace,
            Func<IListMoveOperation<T>, TResult> onMove,
            Func<IListClearOperation<T>, TResult> onClear)
        {
            return onRemove(this);
        }

        void IListOperation<T>.Match(
            Action<IListInsertOperation<T>> onInsert,
            Action<IListRemoveOperation<T>> onRemove,
            Action<IListReplaceOperation<T>> onReplace,
            Action<IListMoveOperation<T>> onMove,
            Action<IListClearOperation<T>> onClear)
        {
            onRemove?.Invoke(this);
        }

        TResult ICollectionOperation<T>.Match<TResult>(
            Func<ICollectionInsertOperation<T>, TResult> onInsert,
            Func<ICollectionRemoveOperation<T>, TResult> onRemove,
            Func<ICollectionReplaceOperation<T>, TResult> onReplace, 
            Func<ICollectionClearOperation<T>, TResult> onClear)
        {
            return onRemove(this);
        }

        void ICollectionOperation<T>.Match(
            Action<ICollectionInsertOperation<T>> onInsert,
            Action<ICollectionRemoveOperation<T>> onRemove,
            Action<ICollectionReplaceOperation<T>> onReplace,
            Action<ICollectionClearOperation<T>> onClear)
        {
            onRemove?.Invoke(this);
        }

        public void MakeImmutable()
        {
        }

        IEnumerable<GeneralChange<T>> IBatch<GeneralChange<T>>.GetPeaces()
        {
            yield return GeneralChange<T>.OnRemove(_item);
        }

        IEnumerable<IndexedChange<T>> IBatch<IndexedChange<T>>.GetPeaces()
        {
            yield return IndexedChange<T>.OnRemove(_item, _index);
        }

        IEnumerable<ICollectionOperation<T>> IBatch<ICollectionOperation<T>>.GetPeaces()
        {
            yield return this;
        }

        IEnumerable<IListOperation<T>> IBatch<IListOperation<T>>.GetPeaces()
        {
            yield return this;
        }
    }
}