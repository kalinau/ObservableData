using System;
using System.Reactive.Linq;
using JetBrains.Annotations;
using ObservableData.Querying;
using ObservableData.Structures.Lists;
using ObservableData.Structures.Lists.Updates;
using ObservableData.Structures.Utils;

namespace ObservableData.Structures
{
    [PublicAPI]
    public static class QueryingPreparer
    {
        [NotNull]
        public static IObservable<IChange<ListOperation<T>>> AsListChanges<T>(
            [NotNull] this IObservableReadOnlyList<T> list) =>
            list.WhenUpdated
                .Select(x => x.NotNull().AsForListQuerying())
                .StartWith(new ListInsertBatchOperation<T>(list, 0))
                .NotNull();

        [NotNull]
        public static IObservable<ListChangePlusState<T>> AsListChangesPlusState<T>(
            [NotNull] this IObservableReadOnlyList<T> list) =>
            list.AsListChanges()
                .Select(x => new ListChangePlusState<T>(x.NotNull(), list))
                .NotNull();

        [NotNull]
        public static IObservable<IChange<CollectionOperation<T>>> AsCollectionChanges<T>(
            [NotNull] this IObservableReadOnlyCollection<T> list) =>
            list.WhenUpdated
                .Select(x => x.NotNull().AsForCollectionQuerying())
                .StartWith(new ListInsertBatchOperation<T>(list, 0))
                .NotNull();

        [NotNull]
        public static IObservable<CollectionChangePlusState<T>> AsCollectionChangesPlusState<T>(
            [NotNull] this IObservableReadOnlyCollection<T> list) =>
            list.AsCollectionChanges()
                .Select(x => new CollectionChangePlusState<T>(x.NotNull(), list))
                .NotNull();

        [NotNull]
        public static IObservable<IChange<ListOperation<T>>> AsListChanges<T>(
            [NotNull] this ObservableList<T> list) =>
            list.WhenUpdated
                .StartWith(new ListInsertBatchOperation<T>(list, 0))
                .NotNull();

        [NotNull]
        public static IObservable<ListChangePlusState<T>> AsListChangesPlusState<T>(
            [NotNull] this ObservableList<T> list) =>
            list.AsListChanges()
                .Select(x => new ListChangePlusState<T>(x.NotNull(), list))
                .NotNull();

        [NotNull]
        public static IObservable<IChange<CollectionOperation<T>>> AsCollectionChanges<T>(
            [NotNull] this ObservableList<T> list) =>
            list.WhenUpdated
                .StartWith(new ListInsertBatchOperation<T>(list, 0))
                .NotNull();

        [NotNull]
        public static IObservable<CollectionChangePlusState<T>> AsCollectionChangesPlusState<T>(
            [NotNull] this ObservableList<T> list) =>
            list.AsCollectionChanges()
                .Select(x => new CollectionChangePlusState<T>(x.NotNull(), list))
                .NotNull();
    }
}