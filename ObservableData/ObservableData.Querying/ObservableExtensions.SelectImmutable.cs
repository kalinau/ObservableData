using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using JetBrains.Annotations;
using ObservableData.Querying.Select;
using ObservableData.Structures;

namespace ObservableData.Querying
{
    public static partial class ObservableExtensions
    {
        public static IObservable<ChangedListData<T>> SelectImmutable<TPrevious, T>(
            [NotNull] this IObservable<ChangedListData<TPrevious>> previous,
            [NotNull] Func<TPrevious, T> func)
        {
            return Observable.Create<ChangedListData<T>>(o =>
            {
                if (o == null) return Disposable.Empty;

                var adapter = new SelectImmutable.ListDataObserver<TPrevious, T>(o, func);
                return previous.Subscribe(adapter);
            });
        }

        public static IObservable<ChangedCollectionData<T>> SelectImmutable<TPrevious, T>(
            [NotNull] this IObservable<IChange<CollectionOperation<TPrevious>>> previous,
            [NotNull] Func<TPrevious, T> func)
        {
            return Observable.Create<ChangedCollectionData<T>>(o =>
            {
                if (o == null) return Disposable.Empty;

                var adapter = new SelectImmutable.CollectionOperationsObserver<TPrevious, T>(o, func);
                return previous.Subscribe(adapter);
            });
        }

        public static IObservable<IChange<ListOperation<T>>> SelectImmutable<TPrevious, T>(
            [NotNull] this IObservable<IChange<ListOperation<TPrevious>>> previous,
            [NotNull] Func<TPrevious, T> func)
        {
            return Observable.Create<IChange<ListOperation<T>>>(o =>
            {
                if (o == null) return Disposable.Empty;

                var adapter = new SelectImmutable.ListOperationsObserver<TPrevious, T>(o, func);
                return previous.Subscribe(adapter);
            });
        }
    }
}