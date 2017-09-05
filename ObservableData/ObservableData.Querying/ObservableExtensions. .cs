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
        public static IObservable<ICollectionChange<T>> StartWithState<T>(
            [NotNull] this IObservable<ICollectionChange<T>> observable,
            [NotNull] IReadOnlyCollection<T> state)
        {
            return observable.StartWith(new CollectionStateChange<T>(state)).NotNull();
        }

        [NotNull]
        public static IObservable<IListChange<T>> StartWithState<T>(
            [NotNull] this IObservable<IListChange<T>> observable,
            [NotNull] IReadOnlyList<T> state)
        {
            return observable.StartWith(new ListStateChange<T>(state)).NotNull();
        }
    }
}