using System;
using JetBrains.Annotations;

namespace ObservableData.Querying.Utils.Adapters
{
    internal abstract class ObserverAdapter<T, TAdaptee> : IObserver<T>
    {
        [NotNull] private readonly IObserver<TAdaptee> _adaptee;

        protected ObserverAdapter([NotNull] IObserver<TAdaptee> adaptee)
        {
            _adaptee = adaptee;
        }

        [NotNull]
        protected IObserver<TAdaptee> Adaptee => _adaptee;

        public void OnCompleted() => _adaptee.OnCompleted();

        public void OnError(Exception error) => _adaptee.OnError(error);

        public abstract void OnNext(T value);
    }

    internal abstract class CollectionChangesObserverAdapter<T, TAdaptee> :
        ObserverAdapter<IChange<CollectionOperation<T>>, IChange<CollectionOperation<TAdaptee>>>
    {
        protected CollectionChangesObserverAdapter([NotNull] IObserver<IChange<CollectionOperation<TAdaptee>>> adaptee) : base(adaptee)
        {
        }
    }

    internal abstract class CollectionChangesObserverAdapter<T> :
        CollectionChangesObserverAdapter<T,T>
    {
        protected CollectionChangesObserverAdapter([NotNull] IObserver<IChange<CollectionOperation<T>>> adaptee) : base(adaptee)
        {
        }
    }

    internal abstract class ListChangesObserverAdapter<T, TAdaptee> :
        ObserverAdapter<IChange<ListOperation<T>>, IChange<ListOperation<TAdaptee>>>
    {
        protected ListChangesObserverAdapter([NotNull] IObserver<IChange<ListOperation<TAdaptee>>> adaptee) : base(adaptee)
        {
        }
    }

    internal abstract class CollectionDataObserverAdapter<T, TAdaptee> :
        ObserverAdapter<CollectionChangePlusState<T>, CollectionChangePlusState<TAdaptee>>
    {
        protected CollectionDataObserverAdapter([NotNull] IObserver<CollectionChangePlusState<TAdaptee>> adaptee) : base(adaptee)
        {
        }
    }

    internal abstract class CollectionDataObserverAdapter<T> : CollectionDataObserverAdapter<T, T>
    {
        protected CollectionDataObserverAdapter([NotNull] IObserver<CollectionChangePlusState<T>> adaptee) : base(adaptee)
        {
        }
    }

    internal abstract class ListDataObserverAdapter<T, TAdaptee> :
        ObserverAdapter<ListChangePlusState<T>, ListChangePlusState<TAdaptee>>
    {
        protected ListDataObserverAdapter([NotNull] IObserver<ListChangePlusState<TAdaptee>> adaptee) : base(adaptee)
        {
        }
    }
}