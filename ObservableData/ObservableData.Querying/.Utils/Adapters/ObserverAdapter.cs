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
        ObserverAdapter<IBatch<GeneralChange<T>>, IBatch<GeneralChange<TAdaptee>>>
    {
        protected CollectionChangesObserverAdapter([NotNull] IObserver<IBatch<GeneralChange<TAdaptee>>> adaptee) : base(adaptee)
        {
        }
    }

    internal abstract class CollectionChangesObserverAdapter<T> :
        CollectionChangesObserverAdapter<T,T>
    {
        protected CollectionChangesObserverAdapter([NotNull] IObserver<IBatch<GeneralChange<T>>> adaptee) : base(adaptee)
        {
        }
    }

    internal abstract class ListChangesObserverAdapter<T, TAdaptee> :
        ObserverAdapter<IBatch<IndexedChange<T>>, IBatch<IndexedChange<TAdaptee>>>
    {
        protected ListChangesObserverAdapter([NotNull] IObserver<IBatch<IndexedChange<TAdaptee>>> adaptee) : base(adaptee)
        {
        }
    }

    internal abstract class CollectionDataObserverAdapter<T, TAdaptee> :
        ObserverAdapter<GeneralChangesPlusState<T>, GeneralChangesPlusState<TAdaptee>>
    {
        protected CollectionDataObserverAdapter([NotNull] IObserver<GeneralChangesPlusState<TAdaptee>> adaptee) : base(adaptee)
        {
        }
    }

    internal abstract class CollectionDataObserverAdapter<T> : CollectionDataObserverAdapter<T, T>
    {
        protected CollectionDataObserverAdapter([NotNull] IObserver<GeneralChangesPlusState<T>> adaptee) : base(adaptee)
        {
        }
    }

    internal abstract class ListDataObserverAdapter<T, TAdaptee> :
        ObserverAdapter<IndexedChangesPlusState<T>, IndexedChangesPlusState<TAdaptee>>
    {
        protected ListDataObserverAdapter([NotNull] IObserver<IndexedChangesPlusState<TAdaptee>> adaptee) : base(adaptee)
        {
        }
    }
}