using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using JetBrains.Annotations;
using ObservableData.Querying.Utils;

namespace ObservableData.Querying
{
    public static partial class ObservableExtensions
    {
        private sealed class ListChangesAdapter<T> : IBatch<GeneralChange<T>>
        {
            [NotNull] private readonly IBatch<IndexedChange<T>> _adaptee;

            public ListChangesAdapter([NotNull] IBatch<IndexedChange<T>> adaptee)
            {
                _adaptee = adaptee;
            }

            public IEnumerable<GeneralChange<T>> GetIterations()
            {
                foreach (var i in _adaptee.GetIterations())
                {
                    foreach (var operation in i.AsCollectionOpperations())
                    {
                        yield return operation;
                    }
                }
            }

            public void MakeImmutable()
            {
                _adaptee.MakeImmutable();
            }
        }

        [NotNull]
        public static IObservable<IBatch<GeneralChange<T>>> AsCollectionChanges<T>(
            [NotNull] this IObservable<IBatch<IndexedChange<T>>> previous)
        {
            return previous.Select(x => x == null ? null : new ListChangesAdapter<T>(x)).NotNull();
        }

        [NotNull]
        public static IObservable<GeneralChangesPlusState<T>> AsCollectionData<T>(
            [NotNull] this IObservable<IndexedChangesPlusState<T>> previous)
        {
            return previous.Select(x =>
                    new GeneralChangesPlusState<T>(
                        new ListChangesAdapter<T>(x.Changes),
                        x.ReachedState))
                .NotNull();
        }

        [NotNull]
        public static IObservable<IBatch<IndexedChange<T>>> AsChanges<T>(
            [NotNull] this IObservable<IndexedChangesPlusState<T>> previous)
        {
            return previous.Select(x => x.Changes).NotNull();
        }

        [NotNull]
        public static IObservable<IBatch<GeneralChange<T>>> AsChanges<T>(
            [NotNull] this IObservable<GeneralChangesPlusState<T>> previous)
        {
            return previous.Select(x => x.Changes).NotNull();
        }
    }
}