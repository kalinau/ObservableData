using ObservableData.Querying;

namespace ObservableData.Structures.Lists.Utils
{
    internal class ListBatchChange<T> : IListChange<T>
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

        public void Enumerate(ICollectionChangeEnumerator<T> enumerator)
        {
            var next = _first;
            while (next != null)
            {
                next.Enumerate(enumerator);
                next = next.Next;
            }
        }

        public void Enumerate(IListChangeEnumerator<T> enumerator)
        {
            var next = _first;
            while (next != null)
            {
                next.Enumerate(enumerator);
                next = next.Next;
            }
        }
    }
}