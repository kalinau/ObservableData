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
        public static IObservable<IChange<ListOperation<T>>> AsListChangesObservable<T>(
            [NotNull] this IObservableReadOnlyList<T> list) =>
            list.WhenUpdated
                .Select(x => x.NotNull().AsForListQuerying())
                .StartWith(new ListInsertBatchOperation<T>(list, 0))
                .NotNull();

        [NotNull]
        public static IObservable<ChangedListData<T>> AsChangedListDataObservable<T>(
            [NotNull] this IObservableReadOnlyList<T> list) =>
            list.AsListChangesObservable()
                .Select(x => new ChangedListData<T>(x.NotNull(), list))
                .NotNull();

        [NotNull]
        public static IObservable<IChange<CollectionOperation<T>>> AsCollectionChangesObservable<T>(
            [NotNull] this IObservableReadOnlyCollection<T> list) =>
            list.WhenUpdated
                .Select(x => x.NotNull().AsForCollectionQuerying())
                .StartWith(new ListInsertBatchOperation<T>(list, 0))
                .NotNull();

        [NotNull]
        public static IObservable<ChangedCollectionData<T>> AsChangedCollectionDataObservable<T>(
            [NotNull] this IObservableReadOnlyCollection<T> list) =>
            list.AsCollectionChangesObservable()
                .Select(x => new ChangedCollectionData<T>(x.NotNull(), list))
                .NotNull();

        [NotNull]
        public static IObservable<IChange<ListOperation<T>>> AsListChangesObservable<T>(
            [NotNull] this ObservableList<T> list) =>
            list.WhenUpdated
                .StartWith(new ListInsertBatchOperation<T>(list, 0))
                .NotNull();

        [NotNull]
        public static IObservable<ChangedListData<T>> AsChangedListDataObservable<T>(
            [NotNull] this ObservableList<T> list) =>
            list.AsListChangesObservable()
                .Select(x => new ChangedListData<T>(x.NotNull(), list))
                .NotNull();

        [NotNull]
        public static IObservable<IChange<CollectionOperation<T>>> AsCollectionChangesObservable<T>(
            [NotNull] this ObservableList<T> list) =>
            list.WhenUpdated
                .StartWith(new ListInsertBatchOperation<T>(list, 0))
                .NotNull();

        [NotNull]
        public static IObservable<ChangedCollectionData<T>> AsChangedCollectionDataObservable<T>(
            [NotNull] this ObservableList<T> list) =>
            list.AsCollectionChangesObservable()
                .Select(x => new ChangedCollectionData<T>(x.NotNull(), list))
                .NotNull();
    }
}