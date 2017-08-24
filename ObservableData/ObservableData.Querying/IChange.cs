using System.Collections.Generic;
using JetBrains.Annotations;

namespace ObservableData.Querying
{
    public interface IChange<out T>
    {
        [NotNull]
        [ItemNotNull]
        IEnumerable<T> GetIterations();

        void MakeImmutable();
    }

    [PublicAPI]
    public static class ChangeExtensions
    {
        public static void ApplyTo<T>(
            [NotNull] this IChange<ListOperation<T>> change,
            [NotNull] IList<T> list)
        {
            foreach (var o in change.GetIterations())
            {
                o.ApplyTo(list);
            }
        }

        public static void ApplyTo<T>(
            [NotNull] this IChange<CollectionOperation<T>> change,
            [NotNull] ICollection<T> collection)
        {
            foreach (var o in change.GetIterations())
            {
                o.ApplyTo(collection);
            }
        }
    }
}