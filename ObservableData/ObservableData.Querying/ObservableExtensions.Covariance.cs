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

            public void MakeImmutable()
            {
                _adaptee.MakeImmutable();
            }

            public IEnumerable<GeneralChange<T>> GetPeaces()
            {
                foreach (var i in _adaptee.GetPeaces())
                {
                    foreach (var change in i.ToGeneralChanges())
                    {
                        yield return change;
                    }
                }
            }
        }

        [NotNull]
        public static IObservable<IBatch<GeneralChange<T>>> SelectGeneralChanges<T>(
            [NotNull] this IObservable<IBatch<IndexedChange<T>>> previous)
        {
            return previous.Select(x => x == null ? null : new ListChangesAdapter<T>(x)).NotNull();
        }

        [NotNull]
        public static IObservable<GeneralChangesPlusState<T>> SelectGeneralChangesPlusState<T>(
            [NotNull] this IObservable<IndexedChangesPlusState<T>> previous)
        {
            return previous.Select(x =>
                    new GeneralChangesPlusState<T>(new ListChangesAdapter<T>(x.Change),x.ReachedState))
                .NotNull();
        }

        [NotNull]
        public static IObservable<IBatch<IndexedChange<T>>> SelectChanges<T>(
            [NotNull] this IObservable<IndexedChangesPlusState<T>> previous)
        {
            return previous.Select(x => x.Change).NotNull();
        }

        [NotNull]
        public static IObservable<IBatch<GeneralChange<T>>> SelectChanges<T>(
            [NotNull] this IObservable<GeneralChangesPlusState<T>> previous)
        {
            return previous.Select(x => x.Changes).NotNull();
        }
    }
}