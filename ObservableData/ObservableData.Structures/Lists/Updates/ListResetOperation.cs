using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using ObservableData.Querying;
using ObservableData.Querying.Utils;

namespace ObservableData.Structures.Lists.Updates
{
    public class ListResetOperation<T> :
        IListChangeNode<T>,
        IListResetOperation<T>, 
        ICollectionResetOperation<T>
    {
        [CanBeNull] private readonly IReadOnlyCollection<T> _items;
        [CanBeNull] private IReadOnlyCollection<T> _locked;

        private readonly ThreadId _threadId;

        public ListResetOperation([CanBeNull] IReadOnlyCollection<T> items)
        {
            _items = items;
            _threadId = ThreadId.FromCurrent();
        }

        IListChangeNode<T> IListChangeNode<T>.Next { get; set; }

        public void MakeImmutable()
        {
            if (_locked == null && _items != null)
            {
                _locked = _items.ToList();
            }
        }

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

        IEnumerable<ListOperation<T>> IChange<ListOperation<T>>.GetIterations()
        {
            yield return ListOperation<T>.OnClear();
            if (this.Items != null)
            {
                var index = 0;
                foreach (var item in this.Items)
                {
                    yield return ListOperation<T>.OnAdd(item, index++);
                }
            }
        }

        IEnumerable<CollectionOperation<T>> IChange<CollectionOperation<T>>.GetIterations()
        {
            yield return CollectionOperation<T>.OnClear(); 
            if (this.Items != null)
            {
                foreach (var item in this.Items)
                {
                    yield return CollectionOperation<T>.OnAdd(item);
                }
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

        TResult IListOperation<T>.Match<TResult>(Func<IListInsertOperation<T>, TResult> onInsert, Func<IListRemoveOperation<T>, TResult> onRemove, Func<IListReplaceOperation<T>, TResult> onReplace, Func<IListMoveOperation<T>, TResult> onMove, Func<IListResetOperation<T>, TResult> onReset)
        {
            return onReset.Invoke(this);
        }

        void IListOperation<T>.Match(Action<IListInsertOperation<T>> onInsert, Action<IListRemoveOperation<T>> onRemove, Action<IListReplaceOperation<T>> onReplace, Action<IListMoveOperation<T>> onMove, Action<IListResetOperation<T>> onReset)
        {
            onReset?.Invoke(this);
        }

        TResult ICollectionOperation<T>.Match<TResult>(Func<ICollectionInsertOperation<T>, TResult> onInsert, Func<ICollectionRemoveOperation<T>, TResult> onRemove, Func<ICollectionReplaceOperation<T>, TResult> onReplace, Func<ICollectionResetOperation<T>, TResult> onReset)
        {
            return onReset.Invoke(this);
        }

        void ICollectionOperation<T>.Match(Action<ICollectionInsertOperation<T>> onInsert, Action<ICollectionRemoveOperation<T>> onRemove, Action<ICollectionReplaceOperation<T>> onReplace, Action<ICollectionResetOperation<T>> onReset)
        {
            onReset?.Invoke(this);
        }
    }
}