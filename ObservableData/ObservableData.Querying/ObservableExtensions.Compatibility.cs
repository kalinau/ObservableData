using System;
using JetBrains.Annotations;
using ObservableData.Querying.Compatibility;
using ObservableData.Querying.Utils;

namespace ObservableData.Querying
{
    public static partial class ObservableExtensions
    {
        //[NotNull]
        //public static IDisposable SubscribeBindableProxy<T>(
        //    [NotNull] this IObservable<IndexedChangesPlusState<T>> observable,
        //    [NotNull] out BindableProxy<T> state)
        //{
        //    var list = new BindableProxy<T>();
        //    state = list;
        //    return observable.Subscribe(x =>
        //    {
        //        list.Subject = x.ReachedState;
        //        x.Change.ApplyTo(list.Events);
        //    }).NotNull();
        //}

        //[NotNull]
        //public static IDisposable SubscribeBindableEvents<T>(
        //    [NotNull] this IObservable<IBatch<IndexedChange<T>>> observable,
        //    [NotNull] out NotifyCollectionEvents<T> events)
        //{
        //    events = new NotifyCollectionEvents<T>();
        //    return observable.SubscribeBindableEvents(events);
        //}

        [NotNull]
        public static IDisposable SubscribeBindableEvents<T>(
            [NotNull] this IObservable<IBatch<IndexedChange<T>>> observable,
            [NotNull] NotifyCollectionEvents<T> events)
        {
            return observable.Subscribe(x => x?.ApplyTo(events)).NotNull();
        }


        public static void ApplyTo<T>(
            [NotNull] this IBatch<IndexedChange<T>> change,
            [NotNull] NotifyCollectionEvents<T> events)
        {
            if (events.HasObservers)
            {
                foreach (var update in change.GetPeaces())
                {
                    events.OnChange(update);
                    if (update.Type == IndexedChangeType.Clear)
                    {
                        break;
                    }
                }
            }
        }
    }
}