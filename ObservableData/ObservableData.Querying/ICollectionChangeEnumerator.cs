namespace ObservableData.Querying
{
    public interface ICollectionChangeEnumerator<in T>
    {
        void OnClear();

        void OnAdd(T item);

        void OnRemove(T item);
    }
}