using System;
using System.Collections.Generic;
using ObservableData.Querying;
using ObservableData.Structures.Lists.Utils;

namespace ObservableData.Structures.Lists.Operations
{
    internal sealed class MoveOperation<T> : 
        IListBatchChangeNode<T>,
        IListMoveOperation<T>
    {
        private readonly int _to;
        private readonly T _item;
        private readonly int _from;

        public MoveOperation(T item, int from, int to)
        {
            _to = to;
            _item = item;
            _from = from;
        }

        IListBatchChangeNode<T> IListBatchChangeNode<T>.Next { get; set; }

        T IListMoveOperation<T>.Item => _item;

        int IListMoveOperation<T>.From => _from;

        int IListMoveOperation<T>.To => _to;

        TResult IListOperation<T>.Match<TResult>(
            Func<IListInsertOperation<T>, TResult> onInsert,
            Func<IListRemoveOperation<T>, TResult> onRemove,
            Func<IListReplaceOperation<T>, TResult> onReplace,
            Func<IListMoveOperation<T>, TResult> onMove,
            Func<IListClearOperation<T>, TResult> onClear)
        {
            return onMove(this);
        }

        void IListOperation<T>.Match(
            Action<IListInsertOperation<T>> onInsert,
            Action<IListRemoveOperation<T>> onRemove,
            Action<IListReplaceOperation<T>> onReplace,
            Action<IListMoveOperation<T>> onMove,
            Action<IListClearOperation<T>> onClear)
        {
            onMove?.Invoke(this);
        }

        public void MakeImmutable()
        {
        }

        IEnumerable<GeneralChange<T>> IBatch<GeneralChange<T>>.GetPeaces()
        {
            yield break;
        }

        IEnumerable<IndexedChange<T>> IBatch<IndexedChange<T>>.GetPeaces()
        {
            yield return IndexedChange<T>.OnMove(_item, _to, _from);
        }

        IEnumerable<IListOperation<T>> IBatch<IListOperation<T>>.GetPeaces()
        {
            yield return this;
        }

        public IEnumerable<ICollectionOperation<T>> GetPeaces()
        {
            yield break;
        }
    }
}