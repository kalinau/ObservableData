using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using JetBrains.Annotations;
using ObservableData.Querying.Utils;
using ObservableData.Querying.Where;

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

                var adapter = new WhereByImmutable.CollectionObserver<T>(o, criterion);
                return previous.Subscribe(adapter);
            }).NotNull();
        }
    }
}