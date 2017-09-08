using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace ObservableData.Querying
{

    public interface IQueryObserver<in T, in TState> : IObserver<T>
    {
        void SetState([NotNull] TState state);
    }

    public interface IObservableCollectionQuery<out T> : IObservable<ICollectionChange<T>>
    {
        IDisposable Subscribe(IQueryObserver<ICollectionChange<T>, IReadOnlyCollection<T>> observer);
    }

    public interface IObservableListQuery<out T> : IObservableCollectionQuery<T> 
    {
        IDisposable Subscribe(IQueryObserver<IListChange<T>, IReadOnlyList<T>> observer);
    }
}