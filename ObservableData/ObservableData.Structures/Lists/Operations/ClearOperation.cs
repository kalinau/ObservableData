using System;
using System.Collections.Generic;
using ObservableData.Querying;
using ObservableData.Structures.Lists.Utils;

namespace ObservableData.Structures.Lists.Operations
{
    public class ClearOperation<T> :
        IListBatchChangeNode<T>
    {
        public static ClearOperation<T> Instance { get; } = new ClearOperation<T>();

        private ClearOperation()
        {
        }

        IListBatchChangeNode<T> IListBatchChangeNode<T>.Next { get; set; }

        public void Enumerate(ICollectionChangeEnumerator<T> enumerator) => enumerator.OnClear();

        public void Enumerate(IListChangeEnumerator<T> enumerator) => enumerator.OnClear();
    }
}