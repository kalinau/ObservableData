using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using ObservableData.Querying.Utils;
using System.Reactive.Linq;
using ObservableData.Querying.Utils.Adapters;

namespace ObservableData.Querying
{
    [PublicAPI]
    public static partial class ObservableExtensions
    {
        [NotNull]
        public static IObservable<IBatch<IndexedChange<T>>> StartWith<T>(
            [NotNull] this IObservable<IBatch<IndexedChange<T>>> observable,
            [NotNull] IReadOnlyCollection<T> state)
        {
            return observable.StartWith(new CurrentStateChange<T>(state)).NotNull();
        }

        [NotNull]
        public static IObservable<IBatch<GeneralChange<T>>> StartWith<T>(
            [NotNull] this IObservable<IBatch<GeneralChange<T>>> observable,
            [NotNull] IReadOnlyCollection<T> state)
        {
            return observable.StartWith(new CurrentStateChange<T>(state)).NotNull();
        }

        [NotNull]
        public static IObservable<IndexedChangesPlusState<T>> WithState<T>(
            [NotNull] this IObservable<IBatch<IndexedChange<T>>> observable,
            [NotNull] IReadOnlyList<T> state)
        {
            return observable.Select(x => new IndexedChangesPlusState<T>(x.NotNull(), state))
                .NotNull();
        }

        [NotNull]
        public static IObservable<GeneralChangesPlusState<T>> WithState<T>(
            [NotNull] this IObservable<IBatch<GeneralChange<T>>> observable,
            [NotNull] IReadOnlyCollection<T> state)
        {
            return observable.Select(x => new GeneralChangesPlusState<T>(x.NotNull(), state))
                .NotNull();
        }
    }
}