using System.Collections.Generic;
using JetBrains.Annotations;
using ObservableData.Querying;
using ObservableData.Structures.Lists.Utils;

namespace ObservableData.Structures.Lists.Operations
{
    internal sealed class InsertBatchOperation<T> :
        IListBatchChangeNode<T>
    {
        private readonly int _index;
        [NotNull] private readonly IReadOnlyCollection<T> _items;

        public InsertBatchOperation([NotNull] IReadOnlyCollection<T> items, int index)
        {
            _items = items;
            _index = index;
        }

        IListBatchChangeNode<T> IListBatchChangeNode<T>.Next { get; set; }

        public void Enumerate(ICollectionChangeEnumerator<T> enumerator)
        {
            foreach (var item in _items)
            {
                enumerator.OnAdd(item);
            }
        }

        public void Enumerate(IListChangeEnumerator<T> enumerator)
        {
            var index = _index;
            foreach (var item in _items)
            {
                enumerator.OnAdd(item, index++);
            }
        }
    }
}