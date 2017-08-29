using System.Collections.Generic;
using JetBrains.Annotations;

namespace ObservableData.Querying
{
    public interface IBatch<out T>
    {
        [NotNull, ItemNotNull]
        IEnumerable<T> GetPeaces();

        void MakeImmutable();
    }
}