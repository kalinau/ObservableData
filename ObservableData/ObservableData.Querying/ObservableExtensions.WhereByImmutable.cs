using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using JetBrains.Annotations;
using ObservableData.Querying.Where;
using ObservableData.Structures;

namespace ObservableData.Querying
{
    public static partial class ObservableExtensions
    {
        public static IObservable<IChange<CollectionOperation<T>>> WhereByImmutable<T>(
            [NotNull] this IObservable<IChange<CollectionOperation<T>>> previous,
            [NotNull] Func<T, bool> criterion)
        {
            return Observable.Create<IChange<CollectionOperation<T>>>(o =>
            {
                if (o == null) return Disposable.Empty;

                var adapter = new WhereByImmutable.CollectionChangesObserver<T>(o, criterion);
                return previous.Subscribe(adapter);
            });
        }
    }
}