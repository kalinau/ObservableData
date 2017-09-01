using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using ObservableData.Querying.Compatibility;

namespace ObservableData.Querying
{
    /// <summary>
    /// Tricky abstraction to enumerate items and do not allocate IEnumerable and/or IEnumerator
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ITrickyEnumerable<out T>
    {
        void Enumerate([NotNull] ITrickyEnumerator<T> handle);
    }

    public interface ITrickyEnumerator<in T>
    {
        bool OnNext(T item);
    }


    public interface ICollectionChange<TItem> : ITrickyEnumerable<GeneralChange<TItem>>
    {
        [CanBeNull]
        IReadOnlyCollection<TItem> State { get; }
    }

    public interface IListChange<TItem> : ITrickyEnumerable<IndexedChange<TItem>>
    {
        [CanBeNull]
        IReadOnlyList<TItem> State { get; }

        //void Match(
        //    Action<IReadOnlyList<TItem>> onStateChanged,
        //    Action<IndexedChange<TItem>> onDelta);
    }

    public interface ICollectionObserver<T> : IObserver<ICollectionChange<T>>
    {
        void OnStart(IReadOnlyCollection<T> collection);
    }

    public interface IBatch<out T>
    {
        [NotNull, ItemNotNull]
        IEnumerable<T> GetPeaces();

        void MakeImmutable();
    }

    public static class BatchExtensions
    {
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