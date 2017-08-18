using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using JetBrains.Annotations;
using ObservableData.Querying.Utils;
using ObservableData.Querying.Where;
using ObservableData.Structures;

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
        public static IObservable<ChangedCollectionData<T>> ForWhereByImmutable<T>(
            [NotNull] this IObservable<ChangedCollectionData<T>> previous,
            [NotNull] Func<T, bool> criterion)
        {
            return Observable.Create<ChangedCollectionData<T>>(o =>
            {
                if (o == null) return Disposable.Empty;

                var adapter = new WhereByImmutable.CollectionDataObserver<T>(o, criterion);
                return previous.Subscribe(adapter);
            }).NotNull();
        }
    }
}