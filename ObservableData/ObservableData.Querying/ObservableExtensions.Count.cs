using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using JetBrains.Annotations;
using ObservableData.Querying.Math;
using ObservableData.Querying.Utils;

namespace ObservableData.Querying
{
    public static partial class ObservableExtensions
    {
        [NotNull]
        public static IObservable<int> CountItems<T>(
            [NotNull] this IObservable<IBatch<GeneralChange<T>>> previous)
        {
            return Observable.Create<int>(observer =>
            {
                if (observer == null) return Disposable.Empty;

                var adapter = new Count.GeneralChangesObserver<T>(observer);
                return previous.Subscribe(adapter);
            }).NotNull();
        }

        [NotNull]
        public static IObservable<int> CountItems<T>(
            [NotNull] this IObservable<IBatch<GeneralChange<T>>> previous, 
            [NotNull] Func<T, bool> criterion)
        {
            return previous.WhereItems(criterion).CountItems();
        }

        [NotNull]
        public static IObservable<int> CountItems<T>(
            [NotNull] this IObservable<IBatch<IndexedChange<T>>> previous)
        {
            return previous.SelectGeneralChanges().CountItems();
        }

        [NotNull]
        public static IObservable<int> CountItems<T>(
            [NotNull] this IObservable<IBatch<IndexedChange<T>>> previous,
            [NotNull] Func<T, bool> selector)
        {
            return previous.SelectGeneralChanges().CountItems();
        }
    }
}