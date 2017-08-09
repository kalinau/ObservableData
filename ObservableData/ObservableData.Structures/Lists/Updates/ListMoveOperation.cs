using System;

namespace ObservableData.Structures.Lists.Updates
{
    public sealed class ListMoveOperation<T> : ListBaseOperation<T>, IListMoveOperation<T>
    {
        private readonly int _from;
        private readonly T _item;
        private readonly int _to;

        public ListMoveOperation(T item, int from, int to)
        {
            _from = from;
            _item = item;
            _to = to;
        }


        public int To => _to;

        public T Item => _item;

        public int From => _from;

        public override void Lock() { }

        public override ICollectionOperation<T> TryGetCollectionOperation() => null;

        public override TResult Match<TResult>(Func<IListInsertOperation<T>, TResult> onInsert, Func<IListRemoveOperation<T>, TResult> onRemove, Func<IListReplaceOperation<T>, TResult> onReplace, Func<IListMoveOperation<T>, TResult> onMove, Func<IListResetOperation<T>, TResult> onReset)
        {
            return onMove(this);
        }

        public override void Match(Action<IListInsertOperation<T>> onInsert, Action<IListRemoveOperation<T>> onRemove, Action<IListReplaceOperation<T>> onReplace, Action<IListMoveOperation<T>> onMove, Action<IListResetOperation<T>> onReset)
        {
            onMove?.Invoke(this);
        }
    }
}
