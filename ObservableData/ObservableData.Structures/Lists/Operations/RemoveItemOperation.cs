using System;
using System.Collections.Generic;
using ObservableData.Querying;
using ObservableData.Structures.Lists.Utils;

namespace ObservableData.Structures.Lists.Operations
{
    internal sealed class RemoveItemOperation<T> : IListBatchChangeNode<T>
    {
        private readonly int _index;
        private readonly T _item;

        public RemoveItemOperation(int index, T item)
        {
            _index = index;
            _item = item;
        }

        IListBatchChangeNode<T> IListBatchChangeNode<T>.Next { get; set; }

        void ICollectionChange<T>.Enumerate(ICollectionChangeEnumerator<T> enumerator) =>
            enumerator.OnRemove(_item, _index);

        void IListChange<T>.Enumerate(IListChangeEnumerator<T> enumerator) => 
            enumerator.OnRemove(_item, _index);
    }
}