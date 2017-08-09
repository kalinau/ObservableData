using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using ObservableData.Structures;

namespace ObservableData.Querying
{
    [PublicAPI]
    public interface IQuery<T>
    {
        void IgnoreEfficiency();

        [NotNull]
        IDisposable Subscribe([NotNull] IObserver<IChange<CollectionOperation<T>>> observer);

        [NotNull]
        IDisposable Subscribe([NotNull] IObserver<IChange<CollectionOperation<T>>> observer, out IReadOnlyCollection<T> mutableState);

        [NotNull]
        IDisposable Subscribe([NotNull] IObserver<IChange<ListOperation<T>>> observer);

        [NotNull]
        IDisposable Subscribe([NotNull] IObserver<IChange<ListOperation<T>>> observer, out IReadOnlyList<T> mutableState);
    }
}
