using ObservableData.Querying.Core;

namespace ObservableData.Structures.Updates
{
    public sealed class ReplaceAtList<T> : ListSingleUpdate<T>
    {
        private readonly T _item;
        private readonly T _changedItem;
        private readonly int _index;

        public ReplaceAtList(T item, T changedItem, int index)
        {
            _item = item;
            _changedItem = changedItem;
            _index = index;
        }

        public override ListOperation<T> First => ListOperation<T>.OnReplace(_item, _changedItem, _index);
    }
}
