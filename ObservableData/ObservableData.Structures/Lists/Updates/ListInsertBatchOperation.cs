using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using ObservableData.Querying;
using ObservableData.Querying.Utils;

namespace ObservableData.Structures.Lists.Updates
{
    public sealed class ListInsertBatchOperation<T> :
        IListChangeNode<T>,
        IListInsertOperation<T>, 
        ICollectionInsertOperation<T>
    {
        private readonly int _index;
        [NotNull] private readonly IReadOnlyCollection<T> _items;
        [CanBeNull] private IReadOnlyCollection<T> _locked;

        private readonly ThreadId _threadId;

        public ListInsertBatchOperation([NotNull] IReadOnlyCollection<T> items, int index)
        {
            _items = items;
            _index = index;
            _threadId = ThreadId.FromCurrent();
        }

        IListChangeNode<T> IListChangeNode<T>.Next { get; set; }

        int IListInsertOperation<T>.Index => _index;

        public IReadOnlyCollection<T> Items
        {
            get
            {
                if (_locked != null)
                {
                    return _locked;
                }
                _threadId.CheckIsCurrent();
                return _items;
            }
        }

        public void MakeImmutable()
        {
            _threadId.CheckIsCurrent();
            if (_locked == null)
            {
                _locked = _items.ToList();
            }
        }

        IEnumerable<ListOperation<T>> IChange<ListOperation<T>>.GetIterations()
        {
            int i = _index;
            foreach (var item in this.Items)
            {
                yield return ListOperation<T>.OnAdd(item, i++);
            }
        }

        IEnumerable<CollectionOperation<T>> IChange<CollectionOperation<T>>.GetIterations()
        {
            foreach (var item in this.Items)
            {
                yield return CollectionOperation<T>.OnAdd(item);
            }
        }

        IEnumerable<IListOperation<T>> IChange<IListOperation<T>>.GetIterations()
        {
            yield return this;
        }

        IEnumerable<ICollectionOperation<T>> IChange<ICollectionOperation<T>>.GetIterations()
        {
            yield return this;
        }

        TResult IListOperation<T>.Match<TResult>(
            Func<IListInsertOperation<T>, TResult> onInsert, 
            Func<IListRemoveOperation<T>, TResult> onRemove, 
            Func<IListReplaceOperation<T>, TResult> onReplace, 
            Func<IListMoveOperation<T>, TResult> onMove,
            Func<IListResetOperation<T>, TResult> onReset)
        {
            return onInsert.Invoke(this);
        }

        void IListOperation<T>.Match(
            Action<IListInsertOperation<T>> onInsert, 
            Action<IListRemoveOperation<T>> onRemove, 
            Action<IListReplaceOperation<T>> onReplace,
            Action<IListMoveOperation<T>> onMove,
            Action<IListResetOperation<T>> onReset)
        {
            onInsert?.Invoke(this);
        }

        TResult ICollectionOperation<T>.Match<TResult>(
            Func<ICollectionInsertOperation<T>, TResult> onInsert, 
            Func<ICollectionRemoveOperation<T>, TResult> onRemove,
            Func<ICollectionReplaceOperation<T>, TResult> onReplace,
            Func<ICollectionResetOperation<T>, TResult> onReset)
        {
            return onInsert.Invoke(this);
        }

        void ICollectionOperation<T>.Match(
            Action<ICollectionInsertOperation<T>> onInsert, 
            Action<ICollectionRemoveOperation<T>> onRemove, 
            Action<ICollectionReplaceOperation<T>> onReplace,
            Action<ICollectionResetOperation<T>> onReset)
        {
            onInsert?.Invoke(this);
        }
    }
}