using System;
using System.Collections.Generic;
using ObservableData.Querying;
using ObservableData.Structures.Lists.Utils;

namespace ObservableData.Structures.Lists.Operations
{
    internal sealed class ReplaceOperation<T> : 
        IListBatchChangeNode<T>,
        IListReplaceOperation<T>, 
        ICollectionReplaceOperation<T>
    {
        private readonly int _index;
        private readonly T _item;
        private readonly T _replacedItem;

        public ReplaceOperation(int index, T item, T replacedItem)
        {
            _index = index;
            _item = item;
            _replacedItem = replacedItem;
        }

        IListBatchChangeNode<T> IListBatchChangeNode<T>.Next { get; set; }

        int IListReplaceOperation<T>.Index => _index;

        T IListReplaceOperation<T>.Item => _item;

        T IListReplaceOperation<T>.ReplacedItem => _replacedItem;

        T ICollectionReplaceOperation<T>.Item => _item;

        T ICollectionReplaceOperation<T>.ReplacedItem => _replacedItem;

        public void MakeImmutable()
        {
        }

        TResult IListOperation<T>.Match<TResult>(
            Func<IListInsertOperation<T>, TResult> onInsert,
            Func<IListRemoveOperation<T>, TResult> onRemove,
            Func<IListReplaceOperation<T>, TResult> onReplace,
            Func<IListMoveOperation<T>, TResult> onMove,
            Func<IListClearOperation<T>, TResult> onClear)
        {
            return onReplace(this);
        }

        void IListOperation<T>.Match(
            Action<IListInsertOperation<T>> onInsert,
            Action<IListRemoveOperation<T>> onRemove,
            Action<IListReplaceOperation<T>> onReplace,
            Action<IListMoveOperation<T>> onMove,
            Action<IListClearOperation<T>> onClear)
        {
            onReplace?.Invoke(this);
        }

        TResult ICollectionOperation<T>.Match<TResult>(Func<ICollectionInsertOperation<T>, TResult> onInsert, Func<ICollectionRemoveOperation<T>, TResult> onRemove, Func<ICollectionReplaceOperation<T>, TResult> onReplace, Func<ICollectionClearOperation<T>, TResult> onClear)
        {
            return onReplace(this);
        }

        void ICollectionOperation<T>.Match(Action<ICollectionInsertOperation<T>> onInsert, Action<ICollectionRemoveOperation<T>> onRemove, Action<ICollectionReplaceOperation<T>> onReplace, Action<ICollectionClearOperation<T>> onClear)
        {
            onReplace?.Invoke(this);
        }

        IEnumerable<GeneralChange<T>> IBatch<GeneralChange<T>>.GetPeaces()
        {
            yield return GeneralChange<T>.OnRemove(_replacedItem);
            yield return GeneralChange<T>.OnAdd(_item);
        }

        IEnumerable<IndexedChange<T>> IBatch<IndexedChange<T>>.GetPeaces()
        {
            yield return IndexedChange<T>.OnReplace(_item, _replacedItem, _index);
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