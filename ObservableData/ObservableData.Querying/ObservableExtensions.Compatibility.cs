using System;
using JetBrains.Annotations;
using ObservableData.Querying.Compatibility;
using ObservableData.Querying.Utils;

namespace ObservableData.Querying
{
    public static partial class ObservableExtensions
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