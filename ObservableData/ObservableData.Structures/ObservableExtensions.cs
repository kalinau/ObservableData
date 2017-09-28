using System;
using System.Collections.Generic;
using System.Linq;
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
            [NotNull] this IObservable<IListBatchChange<T>> observable)
        {
            return observable;
        }

        [NotNull]
        public static IObservable<IBatch<GeneralChange<T>>> SelectGeneralChanges<T>(
            [NotNull] this IObservable<IListBatchChange<T>> observable)
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

        [NotNull]
        public static IObservable<IEnumerable<T>> SelectNewItems<T>(
            [NotNull] this IObservable<IBatch<IListInsertOperation<T>>> observable)
        {
            return observable.Select(GetAdded).NotNull();
        }

        private static IEnumerable<T> GetAdded<T>([NotNull] IBatch<IListInsertOperation<T>> batch)
        {
            foreach (var peace in batch.GetPeaces())
            {
                var e = peace.Match(
                    x => x.NotNull().Items,
                    x => Enumerable.Empty<T>(),
                    x => new[] {x.NotNull().Item},
                    x => Enumerable.Empty<T>(),
                    x => Enumerable.Empty<T>()).NotNull();
                foreach (var item in e)
                {
                    yield return item;
                }
            }
        }
    }
}
