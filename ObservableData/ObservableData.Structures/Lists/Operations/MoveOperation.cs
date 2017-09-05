using System;
using System.Collections.Generic;
using ObservableData.Querying;
using ObservableData.Structures.Lists.Utils;

namespace ObservableData.Structures.Lists.Operations
{
    internal sealed class MoveOperation<T> : 
        IListBatchChangeNode<T>
    {
        private readonly int _to;
        private readonly T _item;
        private readonly int _from;

        public MoveOperation(T item, int from, int to)
        {
            _to = to;
            _item = item;
            _from = from;
        }

        IListBatchChangeNode<T> IListBatchChangeNode<T>.Next { get; set; }

        public void Enumerate(ICollectionChangeEnumerator<T> enumerator) =>
            enumerator.OnMove(_item, _to, _from);

        public void Enumerate(IListChangeEnumerator<T> enumerator) =>
            enumerator.OnMove(_item, _to, _from);
    }
}