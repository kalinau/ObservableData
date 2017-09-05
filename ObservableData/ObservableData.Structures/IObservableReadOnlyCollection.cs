using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using ObservableData.Querying;

namespace ObservableData.Structures
{
    [PublicAPI]
    public interface IObservableReadOnlyCollection<out T> : IReadOnlyCollection<T>
    {
        [NotNull]
        IObservable<ICollectionChange<T>> WhenUpdated { get; }
    }
}