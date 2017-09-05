using System.Collections.Generic;
using JetBrains.Annotations;

namespace ObservableData.Querying.Utils
{
    public static class EmptyList<T>
    {
        [NotNull]
        public static IReadOnlyList<T> Instance { get; } = new T[0];
    }
}
