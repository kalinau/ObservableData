using System;
using JetBrains.Annotations;
using ObservableData.Querying.Compatibility;
using ObservableData.Querying.Utils;

namespace ObservableData.Querying
{
    public static partial class ObservableExtensions
    {
        [NotNull]
        public static IDisposable SubscribeBindableProxy<T>(
            [NotNull] this IObservable<IndexedChangesPlusState<T>> observable,
            [NotNull] out BindableList<T> state)
        {
            var list = new BindableList<T>();
            state = list;
            return observable.Subscribe(x =>
            {
                list.Subject = x.ReachedState;
                foreach (var update in x.Change.GetPeaces())
                {
                    list.Events.OnChange(update);
                    if (update.Type == IndexedChangeType.Clear)
                    {
                        break;
                    }
                }
            }).NotNull();
        }
    }
}