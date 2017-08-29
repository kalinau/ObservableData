using System.Collections.Generic;
using JetBrains.Annotations;
using ObservableData.Querying;

namespace ObservableData.Structures
{
    [PublicAPI]
    public static class BatchExtensions
    {
        [NotNull]
        public static IBatch<IndexedChange<T>> ToIndexedChanges<T>(
            [NotNull] this IBatch<IListOperation<T>> changes)
        {
            return new ListChangesAdapter<T>(changes);
        }

        [NotNull]
        public static IBatch<GeneralChange<T>> ToGeneralChanges<T>(
            [NotNull] this IBatch<IListOperation<T>> changes)
        {
            return new ListChangesAdapter<T>(changes);
        }

        [NotNull]
        public static IBatch<GeneralChange<T>> ToGeneralChanges<T>(
            [NotNull] this IBatch<ICollectionOperation<T>> changes)
        {
            return new CollectionChangesAdapter<T>(changes);
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

            IEnumerable<GeneralChange<T>> IBatch<GeneralChange<T>>.GetPeaces() =>
                _adaptee.GetPeaces().ToGeneralChanges();

            IEnumerable<IndexedChange<T>> IBatch<IndexedChange<T>>.GetPeaces() =>
                _adaptee.GetPeaces().ToIndexedChanges();
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

            public IEnumerable<GeneralChange<T>> GetPeaces() => _adaptee.GetPeaces().ToGeneralChanges();
        }
    }
}