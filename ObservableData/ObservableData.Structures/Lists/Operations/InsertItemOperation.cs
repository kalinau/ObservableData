using System;
using System.Collections;
using System.Collections.Generic;
using ObservableData.Querying;
using ObservableData.Structures.Lists.Utils;

namespace ObservableData.Structures.Lists.Operations
{
    internal sealed class InsertItemOperation<T> : 
        IListBatchChangeNode<T>
    {
        private readonly int _index;
        private readonly T _item;

        public InsertItemOperation(int index, T item)
        {
            _index = index;
            _item = item;
        }

        public IListBatchChangeNode<T> Next { get; set; }

        public void Enumerate(ICollectionChangeEnumerator<T> enumerator) =>
            enumerator.OnAdd(_item);

        public void Enumerate(IListChangeEnumerator<T> enumerator) =>
            enumerator.OnAdd(_item, _index);
    }
}