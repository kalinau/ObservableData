using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using ObservableData.Querying.Utils;

namespace ObservableData.Querying
{
    public static partial class ObservableExtensions
    {
        [NotNull]
        public static IObservable<bool> ContainsItem<T>(
            [NotNull] this IObservable<ICollectionChange<T>> previous,
            T value,
            [NotNull] IEqualityComparer<T> comparer)
        {
            return previous.AnyItem(x => comparer.Equals(x, value));
        }

        [NotNull]
        public static IObservable<bool> ContainsItem<T>(
            [NotNull] this IObservable<ICollectionChange<T>> previous,
            T value)
        {
            return previous.ContainsItem(value, EqualityComparer<T>.Default.NotNull());
        }
    }
}