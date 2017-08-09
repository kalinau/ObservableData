using ObservableData.Querying.Core;

namespace ObservableData.Structures.Updates
{
    public class MoveAtList<T> : ListSingleUpdate<T>
    {
        private readonly T _item;
        private readonly int _to;
        private readonly int _from;

        public MoveAtList(T item, int from, int to)
        {
            _item = item;
            _to = to;
            _from = from;
        }

        public override ListOperation<T> First => ListOperation<T>.OnMove(_item, _to, _from);
    }
}
