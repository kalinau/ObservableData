using System.Collections.Generic;
using JetBrains.Annotations;
using ObservableData.Querying;

namespace ObservableData.Structures.Utils
{
    [PublicAPI]
    public static class ChangeExtensions
    {
        [NotNull]
        public static IChange<ListOperation<T>> AsForListQuerying<T>(
            [NotNull] this IChange<IListOperation<T>> change)
        {
            return new ListChangeAdapter<T>(change);
        }

        [NotNull]
        public static IChange<CollectionOperation<T>> AsForCollectionQuerying<T>(
            [NotNull] this IChange<IListOperation<T>> change)
        {
            return new ListChangeAdapter<T>(change);
        }

        [NotNull]
        public static IChange<CollectionOperation<T>> AsForCollectionQuerying<T>(
            [NotNull] this IChange<ICollectionOperation<T>> change)
        {
            return new CollectionChangeAdapter<T>(change);
        }

        private sealed class ListChangeAdapter<T> :
            IChange<ListOperation<T>>,
            IChange<CollectionOperation<T>>
        {
            [NotNull] private readonly IChange<IListOperation<T>> _adaptee;

            public ListChangeAdapter([NotNull] IChange<IListOperation<T>> adaptee)
            {
                _adaptee = adaptee;
            }

            public void MakeImmutable() => _adaptee.MakeImmutable();

            IEnumerable<CollectionOperation<T>> IChange<CollectionOperation<T>>.GetIterations()
            {
                foreach (var i in _adaptee.GetIterations())
                {
                    foreach (var o in i.AsForCollectionQuerying())
                    {
                        yield return o;
                    }
                }
            }

            IEnumerable<ListOperation<T>> IChange<ListOperation<T>>.GetIterations()
            {
                foreach (var i in _adaptee.GetIterations())
                {
                    var operations = i.AsForListQuerying();
                    foreach (var o in operations)
                    {
                        yield return o;
                    }
                }
            }
        }

        private sealed class CollectionChangeAdapter<T> :
            IChange<CollectionOperation<T>>
        {
            [NotNull] private readonly IChange<ICollectionOperation<T>> _adaptee;

            public CollectionChangeAdapter([NotNull] IChange<ICollectionOperation<T>> adaptee)
            {
                _adaptee = adaptee;
            }

            public void MakeImmutable() => _adaptee.MakeImmutable();

            IEnumerable<CollectionOperation<T>> IChange<CollectionOperation<T>>.GetIterations()
            {
                foreach (var i in _adaptee.GetIterations())
                {
                    foreach (var o in i.AsForCollectionQuerying())
                    {
                        yield return o;
                    }
                }
            }
        }
    }
}
