using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using JetBrains.Annotations;
using ObservableData.Querying.Utils;
using ObservableData.Querying.Where;

namespace ObservableData.Querying
{
    [PublicAPI]
    public static partial class ObservableExtensions
    {
        [NotNull]
        public static IObservable<IChange<CollectionOperation<T>>> ForWhereByImmutable<T>(
            [NotNull] this IObservable<IChange<CollectionOperation<T>>> previous,
            [NotNull] Func<T, bool> criterion)
        {
            return Observable.Create<IChange<CollectionOperation<T>>>(o =>
            {
                if (o == null) return Disposable.Empty;

                var adapter = new WhereByImmutable.CollectionChangesObserver<T>(o, criterion);
                return previous.Subscribe(adapter);
            }).NotNull();
        }

        [NotNull]
        public static IObservable<CollectionChangePlusState<T>> ForWhereByImmutable<T>(
            [NotNull] this IObservable<CollectionChangePlusState<T>> previous,
            [NotNull] Func<T, bool> criterion)
        {
            return Observable.Create<CollectionChangePlusState<T>>(o =>
            {
                if (o == null) return Disposable.Empty;

                var adapter = new WhereByImmutable.CollectionDataObserver<T>(o, criterion);
                return previous.Subscribe(adapter);
            }).NotNull();
        }
    }
}