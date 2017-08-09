using System.Collections.Generic;
using JetBrains.Annotations;

namespace ObservableData.Structures
{
    public interface IChange<out T>
    {
        [NotNull, ItemNotNull]
        IEnumerable<T> Operations();

        void Lock();
    }
}