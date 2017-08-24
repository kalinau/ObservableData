using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using JetBrains.Annotations;
using ObservableData.Querying;
using ObservableData.Structures.Lists.Updates;
using ObservableData.Structures.Utils;

namespace ObservableData.Structures
{
    [PublicAPI]
    public interface IObservableReadOnlyCollection<out T> : IReadOnlyCollection<T>
    {
        [NotNull]
        IObservable<IChange<ICollectionOperation<T>>> WhenUpdated { get; }
    }

    [PublicAPI]
    public static class ReadOnlyCollectionExtensions
    {
    }
}