using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using JetBrains.Annotations;
using ObservableData.Querying.Compatibility;

namespace ObservableData.Querying
{
    /// <summary>
    /// Tricky abstraction to enumerate items and do not allocation IEnumerable and/or IEnumerator
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ITrickyEnumerable<out T>
    {
        void Enumerate([NotNull] Func<T, bool> handle);
    }

    public interface ICollectionChange<TItem>
    {
        IReadOnlyCollection<TItem> TryGetState();

        ITrickyEnumerable<GeneralChange<TItem>> TryGetDelta();
    }

    public interface IListChange<TItem> : ICollectionChange<TItem>
    {
        void Match(
            Action<IReadOnlyList<TItem>> onStateChanged,
            Action<IndexedChange<TItem>> onDelta);
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