using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using ObservableData.Querying;

namespace ObservableData.Structures
{
    [PublicAPI]
    public interface IObservableReadOnlyList<out T> : IReadOnlyList<T>, IObservableReadOnlyCollection<T>
    {
        new int Count { get; }

        [NotNull]
        new IObservable<IChange<IListOperation<T>>> WhenUpdated { get; }
    }
}