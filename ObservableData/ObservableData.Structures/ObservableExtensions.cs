using System;
using System.Reactive.Linq;
using JetBrains.Annotations;
using ObservableData.Querying;
using ObservableData.Structures.Lists;
using ObservableData.Structures.Utils;

namespace ObservableData.Structures
{
    [PublicAPI]
    public static class ObservableExtensions
    {
        [NotNull]
        public static IObservable<IBatch<IndexedChange<T>>> SelectIndexedChanges<T>(
            [NotNull] this IObservable<IListBatch<T>> observable)
        {
            return observable;
        }

        [NotNull]
        public static IObservable<IBatch<GeneralChange<T>>> SelectGeneralChanges<T>(
            [NotNull] this IObservable<IListBatch<T>> observable)
        {
            return observable;
        }

        [NotNull]
        public static IObservable<IBatch<IndexedChange<T>>> SelectIndexedChanges<T>(
            [NotNull] this IObservable<IBatch<IListOperation<T>>> observable)
        {
            return observable.Select(x => x?.ToIndexedChanges()).NotNull();
        }

        [NotNull]
        public static IObservable<IBatch<GeneralChange<T>>> SelectGeneralChanges<T>(
            [NotNull] this IObservable<IBatch<IListOperation<T>>> observable)
        {
            return observable.Select(x => x?.ToGeneralChanges()).NotNull();
        }

        [NotNull]
        public static IObservable<IBatch<GeneralChange<T>>> SelectGeneralChanges<T>(
            [NotNull] this IObservable<IBatch<ICollectionOperation<T>>> observable)
        {
            return observable.Select(x => x?.ToGeneralChanges()).NotNull();
        }
    }
}
