using System.Collections.Generic;
using JetBrains.Annotations;
using ObservableData.Structures;
using ObservableData.Structures.Utils;

namespace ObservableData.Querying.Compatibility
{
    public sealed class CollectionChangeToQueryChanges<T> : IChange<CollectionOperation<T>>
    {
        [NotNull] private readonly IChange<ICollectionOperation<T>> _adaptee;

        public CollectionChangeToQueryChanges([NotNull] IChange<ICollectionOperation<T>> adaptee)
        {
            _adaptee = adaptee;
        }

        public void Lock() => _adaptee.Lock();

        public IEnumerable<CollectionOperation<T>> Operations()
        {
            foreach (var operation in _adaptee.Operations())
            {
                var sum = operation.Match(
                    OnInsert,
                    OnRemove,
                    OnReplace,
                    OnReset
                );
                if (sum.Items != null)
                {
                    foreach (var item in sum.Items)
                    {
                        yield return item;
                    }
                }
                else
                {
                    yield return sum.Item;
                }
            }
        }

        private static ItemOrEnumerable<CollectionOperation<T>> OnInsert([NotNull] ICollectionInsertOperation<T> insert) =>
            new ItemOrEnumerable<CollectionOperation<T>>(Enumerate(insert));

        private static ItemOrEnumerable<CollectionOperation<T>> OnRemove([NotNull] ICollectionRemoveOperation<T> remove) =>
            new ItemOrEnumerable<CollectionOperation<T>>(CollectionOperation<T>.OnRemove(remove.Item));

        private static ItemOrEnumerable<CollectionOperation<T>> OnReplace([NotNull] ICollectionReplaceOperation<T> replace) =>
            new ItemOrEnumerable<CollectionOperation<T>>(Enumerate(replace));

        private static ItemOrEnumerable<CollectionOperation<T>> OnReset([NotNull] ICollectionResetOperation<T> reset) =>
            new ItemOrEnumerable<CollectionOperation<T>>(Enumerate(reset));

        private static IEnumerable<CollectionOperation<T>> Enumerate([NotNull] ICollectionReplaceOperation<T> replace)
        {
            yield return CollectionOperation<T>.OnRemove(replace.ReplacedItem);
            yield return CollectionOperation<T>.OnAdd(replace.Item);
        }

        private static IEnumerable<CollectionOperation<T>> Enumerate([NotNull] ICollectionInsertOperation<T> insert)
        {
            foreach (var item in insert.Items)
            {
                yield return CollectionOperation<T>.OnAdd(item);
            }
        }

        private static IEnumerable<CollectionOperation<T>> Enumerate([NotNull] ICollectionResetOperation<T> reset)
        {
            yield return CollectionOperation<T>.OnClear();
            if (reset.Items != null)
            {
                foreach (var item in reset.Items)
                {
                    yield return CollectionOperation<T>.OnAdd(item);
                }
            }
        }
    }
}