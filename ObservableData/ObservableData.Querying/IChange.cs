using System.Collections.Generic;
using JetBrains.Annotations;

namespace ObservableData.Querying
{
    public interface IChange<out T>
    {
        [NotNull]
        [ItemNotNull]
        IEnumerable<T> GetIterations();

        void MakeImmutable();
    }
}