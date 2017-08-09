using JetBrains.Annotations;

namespace ObservableData.Structures
{
    [PublicAPI]
    public interface IObservableCollection<T> : IObservableReadOnlyCollection<T>, IBatchCollection<T>
    {
        new int Count { get; }

        bool Replace(T oldItem, T newItem);
    }
}