using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using JetBrains.Annotations;
using ObservableData.Querying.Utils;

namespace ObservableData.Querying
{
    public static partial class ObservableExtensions
    {
        [NotNull]
        public static IObservable<ICollectionChange<T>> WhereItems<T>(
            [NotNull] this IObservable<ICollectionChange<T>> previous,
            [NotNull] Func<T, bool> criterion)
        {
            return Observable.Create<ICollectionChange<T>>(o =>
            {
                if (o == null) return Disposable.Empty;

                var enumerator = new Where.ImmutableItemsEnumerator<T>(criterion);
                var adapter = new CollectionObserverWithPreparer<T>(o, enumerator, enumerator);
                return previous.Subscribe(adapter);
            }).NotNull();
        }

        [NotNull]
        public static IObservable<ICollectionChange<T>> WhereItems<T>(
            [NotNull] this IObservable<ICollectionChange<T>> previous,
            [NotNull] Func<T, bool> criterion,
            [NotNull] Func<T, IObservable<bool>> whenStateChanged)
        {
            return Observable.Create<ICollectionChange<T>>(o =>
            {
                if (o == null) return Disposable.Empty;

                var enumerator = new Where.MutableItemsEnumerator<T>(criterion, whenStateChanged);
                var adapter = new CollectionObserverWithPreparer<T>(o, enumerator, enumerator);

                var changesSub = previous.Subscribe(adapter);
                return new CompositeDisposable(changesSub, enumerator);
            }).NotNull();
        }
    }
}