using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace ObservableData.Structures.Lists.Updates
{
    public class ListInsertItemOperation<T> : 
        ListBaseOperation<T>, 
        IListInsertOperation<T>, 
        ICollectionInsertOperation<T>,
        IReadOnlyCollection<T>
    {
        private readonly T _item;
        private readonly int _index;

        public ListInsertItemOperation(T item, int index)
        {
            _item = item;
            _index = index;
        }

        public int Index => _index;

        public IReadOnlyCollection<T> Items => this;

        public override void Lock() { }

        public override TResult Match<TResult>(Func<IListInsertOperation<T>, TResult> onInsert, Func<IListRemoveOperation<T>, TResult> onRemove, Func<IListReplaceOperation<T>, TResult> onReplace, Func<IListMoveOperation<T>, TResult> onMove, Func<IListResetOperation<T>, TResult> onReset)
        {
            return onInsert.Invoke(this);
        }

        public override void Match(Action<IListInsertOperation<T>> onInsert, Action<IListRemoveOperation<T>> onRemove, Action<IListReplaceOperation<T>> onReplace, Action<IListMoveOperation<T>> onMove, Action<IListResetOperation<T>> onReset)
        {
            onInsert?.Invoke(this);
        }

        public TResult Match<TResult>(Func<ICollectionInsertOperation<T>, TResult> onInsert, Func<ICollectionRemoveOperation<T>, TResult> onRemove, Func<ICollectionReplaceOperation<T>, TResult> onReplace, Func<ICollectionResetOperation<T>, TResult> onReset)
        {
            return onInsert.Invoke(this);
        }

        public void Match(Action<ICollectionInsertOperation<T>> onInsert, Action<ICollectionRemoveOperation<T>> onRemove, Action<ICollectionReplaceOperation<T>> onReplace, Action<ICollectionResetOperation<T>> onReset)
        {
            onInsert?.Invoke(this);
        }

        public override ICollectionOperation<T> TryGetCollectionOperation() => this;

        [NotNull]
        private IEnumerable<T> Enumerate()
        {
            yield return _item;
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator() => this.Enumerate().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.Enumerate().GetEnumerator();

        int IReadOnlyCollection<T>.Count => 1;
    }
}