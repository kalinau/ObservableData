using System.Collections.Generic;
using ObservableData.Querying;

namespace ObservableData.Structures.Lists.Utils
{
    internal class ListBatchChange<T> : IListBatchChange<T>
    {
        private IListBatchChangeNode<T> _first;
        private IListBatchChangeNode<T> _last;

        public bool IsReadOnly { get; set; }

        public void Add(IListBatchChangeNode<T> update)
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

        void IBatch<ICollectionOperation<T>>.MakeImmutable()
        {
            var next = _first;
            while (next != null)
            {
                IBatch<ICollectionOperation<T>> changes = next;
                changes.MakeImmutable();
                next = next.Next;
            }
        }

        void IBatch<IListOperation<T>>.MakeImmutable()
        {
            var next = _first;
            while (next != null)
            {
                IBatch<IListOperation<T>> changes = next;
                changes.MakeImmutable();
                next = next.Next;
            }
        }

        void IBatch<GeneralChange<T>>.MakeImmutable()
        {
            var next = _first;
            while (next != null)
            {
                IBatch<GeneralChange<T>> changes = next;
                changes.MakeImmutable();
                next = next.Next;
            }
        }

        void IBatch<IndexedChange<T>>.MakeImmutable()
        {
            var next = _first;
            while (next != null)
            {
                IBatch<IndexedChange<T>> changes = next;
                changes.MakeImmutable();
                next = next.Next;
            }
        }

        IEnumerable<ICollectionOperation<T>> IBatch<ICollectionOperation<T>>.GetPeaces()
        {
            var next = _first;
            while (next != null)
            {
                IBatch<ICollectionOperation<T>> nextBatch = next;
                foreach (var i in nextBatch.GetPeaces())
                {
                    yield return i;
                }
                next = next.Next;
            }
        }

        IEnumerable<IListOperation<T>> IBatch<IListOperation<T>>.GetPeaces()
        {
            var next = _first;
            while (next != null)
            {
                IBatch<IListOperation<T>> nextBatch = next;
                foreach (var i in nextBatch.GetPeaces())
                {
                    yield return i;
                }
                next = next.Next;
            }
        }

        IEnumerable<IndexedChange<T>> IBatch<IndexedChange<T>>.GetPeaces()
        {
            var next = _first;
            while (next != null)
            {
                IBatch<IndexedChange<T>> nextBatch = next;
                foreach (var i in nextBatch.GetPeaces())
                {
                    yield return i;
                }
                next = next.Next;
            }
        }

        IEnumerable<GeneralChange<T>> IBatch<GeneralChange<T>>.GetPeaces()
        {
            var next = _first;
            while (next != null)
            {
                IBatch<GeneralChange<T>> nextBatch = next;
                foreach (var i in nextBatch.GetPeaces())
                {
                    yield return i;
                }
                next = next.Next;
            }
        }
    }
}