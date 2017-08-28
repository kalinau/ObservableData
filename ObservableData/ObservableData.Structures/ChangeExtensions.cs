using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using ObservableData.Querying;
using ObservableData.Structures.Lists;

namespace ObservableData.Structures
{
    [PublicAPI]
    public static class ChangeExtensions
    {
        [NotNull]
        public static IChange<ListOperation<T>> AsQueryingListChange<T>(
            [NotNull] this IChange<IListOperation<T>> change)
        {
            return new ListChangeAdapter<T>(change);
        }

        [NotNull]
        public static IChange<CollectionOperation<T>> AsQueryingCollectionChange<T>(
            [NotNull] this IChange<IListOperation<T>> change)
        {
            return new ListChangeAdapter<T>(change);
        }

        [NotNull]
        public static IChange<CollectionOperation<T>> AsQueryingCollectionChange<T>(
            [NotNull] this IChange<ICollectionOperation<T>> change)
        {
            return new CollectionChangeAdapter<T>(change);
        }

        [NotNull]
        public static IObservable<IChange<ListOperation<T>>> SelectQueryingListChanges<T>(
            [NotNull] this IObservable<IListChange<T>> observable)
        {
            return observable;
        }

        [NotNull]
        public static IObservable<IChange<CollectionOperation<T>>> SelectQueryingCollectionChanges<T>(
            [NotNull] this IObservable<IListChange<T>> observable)
        {
            return observable;
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
                    foreach (var o in i.AsQueryingCollectionOperations())
                    {
                        yield return o;
                    }
                }
            }

            IEnumerable<ListOperation<T>> IChange<ListOperation<T>>.GetIterations()
            {
                foreach (var i in _adaptee.GetIterations())
                {
                    var operations = i.AsQueryingListOperations();
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
                    foreach (var o in i.AsQueryingCollectionOperations())
                    {
                        yield return o;
                    }
                }
            }
        }
    }
}
