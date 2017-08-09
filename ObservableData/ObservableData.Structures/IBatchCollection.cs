using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace ObservableData.Structures
{
    [PublicAPI]
    public interface IBatchCollection<T> : ICollection<T>
    {
        [NotNull]
        IDisposable StartBatchUpdate();

        void Reset([NotNull, InstantHandle] IReadOnlyCollection<T> items);

        void Add([NotNull, InstantHandle] IReadOnlyCollection<T> items);
    }
}