using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace ObservableData.Structures
{
    public interface IObservableReadOnlyCollection<out T> : IReadOnlyCollection<T>
    {
        [NotNull]
        IObservable<IChange<ICollectionOperation<T>>> WhenUpdated { get; }
    }
}