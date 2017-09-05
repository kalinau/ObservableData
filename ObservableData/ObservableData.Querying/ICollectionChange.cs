using System.Collections.Generic;
using System.Threading;
using JetBrains.Annotations;
using ObservableData.Querying.Utils;

namespace ObservableData.Querying
{
    public interface ICollectionChange<out T>
    {
        void Enumerate([NotNull] ICollectionChangeEnumerator<T> enumerator);
    }

    public static class CollectionChangExtensions
    {
        private sealed class ToCollectionApplier<T> : ICollectionChangeEnumerator<T>
        {
            [NotNull]
            private static readonly ThreadLocal<ToCollectionApplier<T>> Factory =
                new ThreadLocal<ToCollectionApplier<T>>(() => new ToCollectionApplier<T>());

            private ICollection<T> _collection;

            [NotNull]
            public static ToCollectionApplier<T> GetInstance([NotNull] ICollection<T> collection)
            {
                var result = Factory.Value.NotNull();
                result.SetList(collection);
                return result;
            }
            private void SetList(ICollection<T> list)
            {
                _collection = list;
            }

            public void OnStateChanged(IReadOnlyCollection<T> state)
            {
                var collection = _collection.Check();
                collection.Clear();
                foreach (var item in state)
                {
                    collection.Add(item);
                }
            }

            public void OnClear() => _collection.Check().Clear();

            public void OnAdd(T item) => _collection.Check().Add(item);

            public void OnRemove(T item) => _collection.Check().Remove(item);
        }

        public static void ApplyTo<T>(
            [NotNull] this ICollectionChange<T> change,
            [NotNull] ICollection<T> list)
        {
            var applier = ToCollectionApplier<T>.GetInstance(list);
            change.Enumerate(applier);
        }
    }
}