using System.Collections.Generic;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace ObservableData.Querying
{
    public interface IBatch<out T>
    {
        [NotNull]
        [ItemNotNull]
        IEnumerable<T> GetIterations();

        void MakeImmutable();
    }
}