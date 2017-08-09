using ObservableData.Querying.Core;

namespace ObservableData.Structures.Updates
{
    public class AddItemToList<T> : ListSingleUpdate<T>
    {
        private readonly T _item;
        private readonly int _index;

        public AddItemToList(T item, int index)
        {
            _item = item;
            _index = index;
        }

        public override ListOperation<T> First => ListOperation<T>.OnAdd(_item, _index);
    }
}
