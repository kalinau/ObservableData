using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace ObservableData.Querying
{
    public interface IQueryObserver<in T> : IObserver<T>
    {
        bool IsMultiTimesEnumerator { get; }
    }

    public interface IQueryStateObserver<in T, in TState> : IQueryObserver<T>
    {
        void OnStart([CanBeNull] TState state);
    }

    public interface IObservableQuery<out T> : IObservable<ICollectionChange<T>>
    {
        IDisposable Subscribe(IQueryObserver<ICollectionChange<T>> observer);
    }

    public interface IObservableCollectionState<out T> : IObservableQuery<ICollectionChange<T>>
    {
        IDisposable Subscribe(
            IQueryObserver<ICollectionChange<T>> observer,
            out IReadOnlyCollection<T> state);
    }

    public interface IObservableCollectionQuery<out T> : IObservable<ICollectionChange<T>>
    {
        IDisposable Subscribe(IQueryStateObserver<ICollectionChange<T>, IReadOnlyCollection<T>> stateStateObserver);
    }

    public interface IObservableListQuery<out T> : IObservableCollectionQuery<T> 
    {
        IDisposable Subscribe(IQueryStateObserver<IListChange<T>, IReadOnlyList<T>> stateStateObserver);
    }
}