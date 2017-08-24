using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using ObservableData.Querying;
using ObservableData.Querying.Compatibility;
using ObservableData.Structures.Utils;

namespace ObservableData.Structures
{
    [PublicAPI]
    public interface IObservableReadOnlyList<out T> : IReadOnlyList<T>, IObservableReadOnlyCollection<T>
    {
        new int Count { get; }

        [NotNull]
        new IObservable<IChange<IListOperation<T>>> WhenUpdated { get; }
    }

    [PublicAPI]
    public static class ReadOnlyListExtensions
    {
        public static IDisposable ToBindableList<T>(
            [NotNull] this IObservableReadOnlyList<T> source,
            [NotNull] out BindableList<T> result)
        {
            var list = new BindableList<T>();
            list.Subject = source;
            result = list;
            return source.WhenUpdated.Subscribe(x =>
            {
                if (x == null) return;

                foreach (var update in x.AsForListQuerying().GetIterations())
                {
                    list.Events.OnOperation(update);
                    if (update.Type == ListOperationType.Clear)
                    {
                        break;
                    }
                }
            }).NotNull();
        }
    }
}