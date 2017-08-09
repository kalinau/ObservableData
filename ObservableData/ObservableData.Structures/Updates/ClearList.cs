using ObservableData.Querying.Core;

namespace ObservableData.Structures.Updates
{
    public class ClearList<T> : ListSingleUpdate<T>
    {
        public static ClearList<T> Instance { get; } = new ClearList<T>();

        private ClearList()
        {
        }

        public override ListOperation<T> First => ListOperation<T>.OnClear();
    }
}
