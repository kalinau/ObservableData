﻿using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using ObservableData.Querying.Utils;

namespace ObservableData.Structures.Collections.Updates
{
    public class CollectionInsertBatchOperation<T> :
        ICollectionInsertOperation<T>
    {
        [NotNull] private readonly IReadOnlyCollection<T> _items;
        [CanBeNull] private IReadOnlyCollection<T> _locked;

        private readonly ThreadId _threadId;

        public CollectionInsertBatchOperation([NotNull] IReadOnlyCollection<T> items)
        {
            _items = items;
            _threadId = ThreadId.FromCurrent();
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

        public void MakeImmutable()
        {
            if (_locked == null)
            {
                _locked = _items.ToList();
            }
        }

        public TResult Match<TResult>(Func<ICollectionInsertOperation<T>, TResult> onInsert, Func<ICollectionRemoveOperation<T>, TResult> onRemove, Func<ICollectionReplaceOperation<T>, TResult> onReplace, Func<ICollectionClearOperation<T>, TResult> onClear)
        {
            return onInsert.Invoke(this);
        }

        public void Match(Action<ICollectionInsertOperation<T>> onInsert, Action<ICollectionRemoveOperation<T>> onRemove, Action<ICollectionReplaceOperation<T>> onReplace, Action<ICollectionClearOperation<T>> onClear)
        {
            onInsert?.Invoke(this);
        }
    }
}
