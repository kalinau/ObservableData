using ObservableData.Querying.Core;

namespace ObservableData.Structures.Updates
{
    public sealed class RemoveAtList<T> : ListSingleUpdate<T>
    {
        private readonly T _item;
        private readonly int _index;

        public RemoveAtList(T item, int index)
        {
            _item = item;
            _index = index;
        }

        public override ListOperation<T> First => ListOperation<T>.OnRemove(_item, _index);
    }
}
