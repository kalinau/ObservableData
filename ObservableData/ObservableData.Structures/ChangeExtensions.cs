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
        public static IBatch<IndexedChange<T>> AsQueryingListChange<T>(
            [NotNull] this IBatch<IListOperation<T>> changes)
        {
            return new ListChangesAdapter<T>(changes);
        }

        [NotNull]
        public static IBatch<GeneralChange<T>> AsQueryingCollectionChange<T>(
            [NotNull] this IBatch<IListOperation<T>> changes)
        {
            return new ListChangesAdapter<T>(changes);
        }

        [NotNull]
        public static IBatch<GeneralChange<T>> AsQueryingCollectionChange<T>(
            [NotNull] this IBatch<ICollectionOperation<T>> changes)
        {
            return new CollectionChangesAdapter<T>(changes);
        }

        [NotNull]
        public static IObservable<IBatch<IndexedChange<T>>> SelectQueryingListChanges<T>(
            [NotNull] this IObservable<IListBatch<T>> observable)
        {
            return observable;
        }

        [NotNull]
        public static IObservable<IBatch<GeneralChange<T>>> SelectQueryingCollectionChanges<T>(
            [NotNull] this IObservable<IListBatch<T>> observable)
        {
            return observable;
        }

        private sealed class ListChangesAdapter<T> :
            IBatch<IndexedChange<T>>,
            IBatch<GeneralChange<T>>
        {
            [NotNull] private readonly IBatch<IListOperation<T>> _adaptee;

            public ListChangesAdapter([NotNull] IBatch<IListOperation<T>> adaptee)
            {
                _adaptee = adaptee;
            }

            public void MakeImmutable() => _adaptee.MakeImmutable();

            IEnumerable<GeneralChange<T>> IBatch<GeneralChange<T>>.GetIterations()
            {
                foreach (var i in _adaptee.GetIterations())
                {
                    foreach (var o in i.AsQueryingCollectionOperations())
                    {
                        yield return o;
                    }
                }
            }

            IEnumerable<IndexedChange<T>> IBatch<IndexedChange<T>>.GetIterations()
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

        private sealed class CollectionChangesAdapter<T> :
            IBatch<GeneralChange<T>>
        {
            [NotNull] private readonly IBatch<ICollectionOperation<T>> _adaptee;

            public CollectionChangesAdapter([NotNull] IBatch<ICollectionOperation<T>> adaptee)
            {
                _adaptee = adaptee;
            }

            public void MakeImmutable() => _adaptee.MakeImmutable();

            IEnumerable<GeneralChange<T>> IBatch<GeneralChange<T>>.GetIterations()
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
