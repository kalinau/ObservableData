using System.Collections.Generic;
using ObservableData.Querying;

namespace ObservableData.Structures.Lists.Updates
{
    internal class ListBatchChange<T> : IListChange<T>
    {
        private IListChangeNode<T> _first;
        private IListChangeNode<T> _last;

        public void Add(IListChangeNode<T> update)
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


        void IChange<ICollectionOperation<T>>.MakeImmutable()
        {
            var next = _first;
            while (next != null)
            {
                IChange<ICollectionOperation<T>> change = next;
                change.MakeImmutable();
                next = next.Next;
            }
        }

        void IChange<IListOperation<T>>.MakeImmutable()
        {
            var next = _first;
            while (next != null)
            {
                IChange<IListOperation<T>> change = next;
                change.MakeImmutable();
                next = next.Next;
            }
        }

        void IChange<CollectionOperation<T>>.MakeImmutable()
        {
            var next = _first;
            while (next != null)
            {
                IChange<CollectionOperation<T>> change = next;
                change.MakeImmutable();
                next = next.Next;
            }
        }

        void IChange<ListOperation<T>>.MakeImmutable()
        {
            var next = _first;
            while (next != null)
            {
                IChange<ListOperation<T>> change = next;
                change.MakeImmutable();
                next = next.Next;
            }
        }


        IEnumerable<ICollectionOperation<T>> IChange<ICollectionOperation<T>>.GetIterations()
        {
            var next = _first;
            while (next != null)
            {
                IChange<ICollectionOperation<T>> change = next;
                foreach (var i in change.GetIterations())
                {
                    yield return i;
                }
                next = next.Next;
            }
        }

        IEnumerable<IListOperation<T>> IChange<IListOperation<T>>.GetIterations()
        {
            var next = _first;
            while (next != null)
            {
                IChange<IListOperation<T>> change = next;
                foreach (var i in change.GetIterations())
                {
                    yield return i;
                }
                next = next.Next;
            }
        }

        IEnumerable<ListOperation<T>> IChange<ListOperation<T>>.GetIterations()
        {
            var next = _first;
            while (next != null)
            {
                IChange<ListOperation<T>> change = next;
                foreach (var i in change.GetIterations())
                {
                    yield return i;
                }
                next = next.Next;
            }
        }

        IEnumerable<CollectionOperation<T>> IChange<CollectionOperation<T>>.GetIterations()
        {
            var next = _first;
            while (next != null)
            {
                IChange<CollectionOperation<T>> change = next;
                foreach (var i in change.GetIterations())
                {
                    yield return i;
                }
                next = next.Next;
            }
        }
    }
}