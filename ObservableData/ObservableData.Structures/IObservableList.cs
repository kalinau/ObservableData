using System.Collections.Generic;

namespace ObservableData.Structures
{
    public interface IObservableList<T> : IObservableReadOnlyList<T>, IList<T>, IObservableCollection<T>
    {
        void Move(int from, int to);

        new T this[int index] { get; set; }

        new int Count { get; }
    }
}