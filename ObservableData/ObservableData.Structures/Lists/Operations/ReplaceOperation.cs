using ObservableData.Querying;
using ObservableData.Structures.Lists.Utils;

namespace ObservableData.Structures.Lists.Operations
{
    internal sealed class ReplaceOperation<T> : 
        IListBatchChangeNode<T>
    {
        private readonly int _index;
        private readonly T _item;
        private readonly T _replacedItem;

        public ReplaceOperation(int index, T item, T replacedItem)
        {
            _index = index;
            _item = item;
            _replacedItem = replacedItem;
        }

        IListBatchChangeNode<T> IListBatchChangeNode<T>.Next { get; set; }

        void ICollectionChange<T>.Enumerate(ICollectionChangeEnumerator<T> enumerator) =>
            enumerator.OnReplace(_item, _replacedItem, _index);

        public void Enumerate(IListChangeEnumerator<T> enumerator) =>
            enumerator.OnReplace(_item, _replacedItem, _index);
    }
}