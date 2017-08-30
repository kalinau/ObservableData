using System;
using System.Collections;
using System.Collections.Generic;
using ObservableData.Querying;
using ObservableData.Structures.Lists.Utils;

namespace ObservableData.Structures.Lists.Operations
{
    internal sealed class InsertItemOperation<T> : 
        IListBatchChangeNode<T>,
        IListInsertOperation<T>, 
        ICollectionInsertOperation<T>,
        IReadOnlyCollection<T>
    {
        private readonly int _index;
        private readonly T _item;

        public InsertItemOperation(int index, T item)
        {
            _index = index;
            _item = item;
        }

        public IListBatchChangeNode<T> Next { get; set; }

        int IListInsertOperation<T>.Index => _index;

        IReadOnlyCollection<T> IListInsertOperation<T>.Items => this;

        IReadOnlyCollection<T> ICollectionInsertOperation<T>.Items => this;

        TResult IListOperation<T>.Match<TResult>(
            Func<IListInsertOperation<T>, TResult> onInsert,
            Func<IListRemoveOperation<T>, TResult> onRemove,
            Func<IListReplaceOperation<T>, TResult> onReplace,
            Func<IListMoveOperation<T>, TResult> onMove,
            Func<IListClearOperation<T>, TResult> onClear)
        {
            return onInsert(this);
        }

        void IListOperation<T>.Match(
            Action<IListInsertOperation<T>> onInsert,
            Action<IListRemoveOperation<T>> onRemove,
            Action<IListReplaceOperation<T>> onReplace,
            Action<IListMoveOperation<T>> onMove,
            Action<IListClearOperation<T>> onClear)
        {
            onInsert?.Invoke(this);
        }

        TResult ICollectionOperation<T>.Match<TResult>(
            Func<ICollectionInsertOperation<T>, TResult> onInsert,
            Func<ICollectionRemoveOperation<T>, TResult> onRemove,
            Func<ICollectionReplaceOperation<T>, TResult> onReplace, 
            Func<ICollectionClearOperation<T>, TResult> onClear)
        {
            return onInsert(this);
        }

        void ICollectionOperation<T>.Match(
            Action<ICollectionInsertOperation<T>> onInsert,
            Action<ICollectionRemoveOperation<T>> onRemove,
            Action<ICollectionReplaceOperation<T>> onReplace,
            Action<ICollectionClearOperation<T>> onClear)
        {
            onInsert?.Invoke(this);
        }

        public void MakeImmutable()
        {
        }

        IEnumerable<GeneralChange<T>> IBatch<GeneralChange<T>>.GetPeaces()
        {
            yield return GeneralChange<T>.OnAdd(_item);
        }

        IEnumerable<IndexedChange<T>> IBatch<IndexedChange<T>>.GetPeaces()
        {
            yield return IndexedChange<T>.OnAdd(_item, _index);
        }

        IEnumerable<ICollectionOperation<T>> IBatch<ICollectionOperation<T>>.GetPeaces()
        {
            yield return this;
        }

        IEnumerable<IListOperation<T>> IBatch<IListOperation<T>>.GetPeaces()
        {
            yield return this;
        }


        int IReadOnlyCollection<T>.Count => 1;

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            yield return _item;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            yield return _item;
        }
    }
}