using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using ObservableData.Querying;
using ObservableData.Querying.Utils;
using ObservableData.Structures.Lists.Utils;

namespace ObservableData.Structures.Lists.Operations
{
    internal sealed class InsertBatchOperation<T> :
        IListBatchChangeNode<T>,
        IListInsertOperation<T>, 
        ICollectionInsertOperation<T>
    {
        private readonly int _index;
        [NotNull] private readonly IReadOnlyCollection<T> _items;
        [CanBeNull] private IReadOnlyCollection<T> _locked;

        private readonly ThreadId _threadId;

        public InsertBatchOperation([NotNull] IReadOnlyCollection<T> items, int index)
        {
            _items = items;
            _index = index;
            _threadId = ThreadId.FromCurrent();
        }

        IListBatchChangeNode<T> IListBatchChangeNode<T>.Next { get; set; }

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

        IEnumerable<IndexedChange<T>> IBatch<IndexedChange<T>>.GetPeaces()
        {
            var index = _index;
            foreach (var item in _items)
            {
                yield return IndexedChange<T>.OnAdd(item, index++);
            }
        }

        IEnumerable<GeneralChange<T>> IBatch<GeneralChange<T>>.GetPeaces()
        {
            foreach (var item in _items)
            {
                yield return GeneralChange<T>.OnAdd(item);
            }
        }

        IEnumerable<IListOperation<T>> IBatch<IListOperation<T>>.GetPeaces()
        {
            yield return this;
        }

        IEnumerable<ICollectionOperation<T>> IBatch<ICollectionOperation<T>>.GetPeaces()
        {
            yield return this;
        }

        TResult IListOperation<T>.Match<TResult>(
            Func<IListInsertOperation<T>, TResult> onInsert, 
            Func<IListRemoveOperation<T>, TResult> onRemove, 
            Func<IListReplaceOperation<T>, TResult> onReplace, 
            Func<IListMoveOperation<T>, TResult> onMove,
            Func<IListClearOperation<T>, TResult> onClear)
        {
            return onInsert.Invoke(this);
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
            return onInsert.Invoke(this);
        }

        void ICollectionOperation<T>.Match(
            Action<ICollectionInsertOperation<T>> onInsert, 
            Action<ICollectionRemoveOperation<T>> onRemove, 
            Action<ICollectionReplaceOperation<T>> onReplace,
            Action<ICollectionClearOperation<T>> onClear)
        {
            onInsert?.Invoke(this);
        }
    }
}