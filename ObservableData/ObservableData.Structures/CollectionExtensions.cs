using System;
using System.Reactive.Linq;
using JetBrains.Annotations;
using ObservableData.Structures.Collections.Updates;
using ObservableData.Structures.Utils;

namespace ObservableData.Structures
{
    public static class CollectionExtensions
    {
        [NotNull]
        public static IObservable<IChange<ICollectionOperation<T>>> AsObservable<T>(
            [NotNull] this IObservableReadOnlyCollection<T> list) =>
            list.WhenUpdated.StartWith(new CollectionInsertBatchOperation<T>(list)).NotNull();
    }
}
