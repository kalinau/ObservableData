using System;

namespace ObservableData.Structures.Lists.Updates
{
    public class ListRemoveOperation<T> : ListBaseOperation<T>, IListRemoveOperation<T>, ICollectionRemoveOperation<T>
    {
        private readonly int _index;
        private readonly T _item;

        public ListRemoveOperation(T item, int index)
        {
            _index = index;
            _item = item;
        }

        public override ICollectionOperation<T> TryGetCollectionOperation() => this;

        public int Index => _index;

        T IListRemoveOperation<T>.Item => _item;

        T ICollectionRemoveOperation<T>.Item => _item;

        public override void Lock()
        {
        }

        public override TResult Match<TResult>(Func<IListInsertOperation<T>, TResult> onInsert, Func<IListRemoveOperation<T>, TResult> onRemove, Func<IListReplaceOperation<T>, TResult> onReplace, Func<IListMoveOperation<T>, TResult> onMove, Func<IListResetOperation<T>, TResult> onReset)
        {
            return onRemove.Invoke(this);
        }

        public override void Match(Action<IListInsertOperation<T>> onInsert, Action<IListRemoveOperation<T>> onRemove, Action<IListReplaceOperation<T>> onReplace, Action<IListMoveOperation<T>> onMove, Action<IListResetOperation<T>> onReset)
        {
            onRemove?.Invoke(this);
        }

        public TResult Match<TResult>(Func<ICollectionInsertOperation<T>, TResult> onInsert, Func<ICollectionRemoveOperation<T>, TResult> onRemove, Func<ICollectionReplaceOperation<T>, TResult> onReplace, Func<ICollectionResetOperation<T>, TResult> onReset)
        {
            return onRemove.Invoke(this);
        }

        public void Match(Action<ICollectionInsertOperation<T>> onInsert, Action<ICollectionRemoveOperation<T>> onRemove, Action<ICollectionReplaceOperation<T>> onReplace, Action<ICollectionResetOperation<T>> onReset)
        {
            onRemove?.Invoke(this);
        }
    }
}
