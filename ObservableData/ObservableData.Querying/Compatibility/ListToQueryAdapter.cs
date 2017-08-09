using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using JetBrains.Annotations;
using ObservableData.Querying.Utils;
using ObservableData.Structures;

namespace ObservableData.Querying.Compatibility
{
    public class ListToQueryAdapter<T> : IQuery<T>
    {
        [NotNull] private readonly IObservableReadOnlyList<T> _adaptee;

        public ListToQueryAdapter([NotNull] IObservableReadOnlyList<T> adaptee)
        {
            _adaptee = adaptee;
        }

        public void IgnoreEfficiency()
        {
        }

        public IDisposable Subscribe(IObserver<IChange<CollectionOperation<T>>> observer)
        {
            IObservableReadOnlyCollection<T> collection = _adaptee;
            return collection.AsObservable().Select(Adapt).NotNull().Subscribe(observer).NotNull();
        }

        public IDisposable Subscribe(IObserver<IChange<ListOperation<T>>> observer)
        {
            return _adaptee.AsObservable().Select(Adapt).NotNull().Subscribe(observer).NotNull();
        }

        public IDisposable Subscribe(IObserver<IChange<CollectionOperation<T>>> observer, out IReadOnlyCollection<T> mutableState)
        {
            IObservableReadOnlyCollection<T> collection = _adaptee;
            mutableState = collection;
            return collection.WhenUpdated.Select(Adapt).NotNull().Subscribe(observer).NotNull();
        }

        public IDisposable Subscribe(IObserver<IChange<ListOperation<T>>> observer, out IReadOnlyList<T> mutableState)
        {
            mutableState = _adaptee;
            return _adaptee.WhenUpdated.Select(Adapt).NotNull().Subscribe(observer).NotNull();
        }

        private static ListChangeToQueryChanges<T> Adapt([NotNull] IChange<IListOperation<T>> x)
        {
            return new ListChangeToQueryChanges<T>(x);
        }

        private static CollectionChangeToQueryChanges<T> Adapt([NotNull] IChange<ICollectionOperation<T>> x)
        {
            return new CollectionChangeToQueryChanges<T>(x);
        }
    }
}
