using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace ObservableData.Structures.Lists.Updates
{
    public class ListBatchChange<T> : IDisposable, IListChange<T>
    {
        [NotNull] private readonly Action<ListBatchChange<T>> _onDispose;
        private ListBaseOperation<T> _first;
        private ListBaseOperation<T> _last;

        public ListBatchChange([NotNull] Action<ListBatchChange<T>> onDispose)
        {
            _onDispose = onDispose;
        }

        public void Add(ListBaseOperation<T> update)
        {
            if (_last == null)
            {
                _first = update;
                _last = update;
            }
            else
            {
                _last.Next = update;
                _last = update;
            }
        }

        public void Clear()
        {
            _last = null;
            _first = null;
        }

        public void Dispose()
        {
            _onDispose(this);
        }

        public void Lock()
        {
            var next = _first;
            while (next != null)
            {
                next.Lock();
                next = next.Next;
            }
        }

        IEnumerable<ICollectionOperation<T>> IChange<ICollectionOperation<T>>.Operations()
        {
            var next = _first;
            while (next != null)
            {
                var operation = next.TryGetCollectionOperation();
                if (operation != null)
                {
                    yield return operation;
                }
                next = next.Next;
            }
        }

        IEnumerable<IListOperation<T>> IChange<IListOperation<T>>.Operations()
        {
            var next = _first;
            while (next != null)
            {
                yield return next;
                next = next.Next;
            }
        }
    }
}
