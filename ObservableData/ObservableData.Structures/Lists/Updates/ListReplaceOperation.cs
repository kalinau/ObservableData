using System;

namespace ObservableData.Structures.Lists.Updates
{
    public sealed class ListReplaceOperation<T> : ListBaseOperation<T>, IListReplaceOperation<T>, ICollectionReplaceOperation<T>
    {
        private readonly int _index;
        private readonly T _item;
        private readonly T _replacedItem;

        public ListReplaceOperation(int index, T item, T replacedItem)
        {
            _index = index;
            _item = item;
            _replacedItem = replacedItem;
        }


        public int Index => _index;

        public T Item => _item;

        public T ReplacedItem => _replacedItem;

        public override void Lock() { }

        public override ICollectionOperation<T> TryGetCollectionOperation() => this;

        public override TResult Match<TResult>(Func<IListInsertOperation<T>, TResult> onInsert, Func<IListRemoveOperation<T>, TResult> onRemove, Func<IListReplaceOperation<T>, TResult> onReplace, Func<IListMoveOperation<T>, TResult> onMove, Func<IListResetOperation<T>, TResult> onReset)
        {
            return onReplace(this);
        }

        public override void Match(Action<IListInsertOperation<T>> onInsert, Action<IListRemoveOperation<T>> onRemove, Action<IListReplaceOperation<T>> onReplace, Action<IListMoveOperation<T>> onMove, Action<IListResetOperation<T>> onReset)
        {
            onReplace?.Invoke(this);
        }

        public TResult Match<TResult>(Func<ICollectionInsertOperation<T>, TResult> onInsert, Func<ICollectionRemoveOperation<T>, TResult> onRemove, Func<ICollectionReplaceOperation<T>, TResult> onReplace, Func<ICollectionResetOperation<T>, TResult> onReset)
        {
            return onReplace(this);
        }

        public void Match(Action<ICollectionInsertOperation<T>> onInsert, Action<ICollectionRemoveOperation<T>> onRemove, Action<ICollectionReplaceOperation<T>> onReplace, Action<ICollectionResetOperation<T>> onReset)
        {
            onReplace?.Invoke(this);
        }
    }
}
