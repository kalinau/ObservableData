using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using JetBrains.Annotations;
using ObservableData.Querying.Utils;

namespace ObservableData.Querying
{
    public static partial class ObservableExtensions
    {
        private sealed class ListChangeAdapter<T> : IChange<CollectionOperation<T>>
        {
            [NotNull] private readonly IChange<ListOperation<T>> _adaptee;

            public ListChangeAdapter([NotNull] IChange<ListOperation<T>> adaptee)
            {
                _adaptee = adaptee;
            }

            public IEnumerable<CollectionOperation<T>> GetIterations()
            {
                foreach (var i in _adaptee.GetIterations())
                {
                    foreach (var operation in i.AsForCollection())
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
        public static IObservable<IChange<CollectionOperation<T>>> AsCollectionChanges<T>(
            [NotNull] this IObservable<IChange<ListOperation<T>>> previous)
        {
            return previous.Select(x => x == null ? null : new ListChangeAdapter<T>(x)).NotNull();
        }

        [NotNull]
        public static IObservable<ChangedCollectionData<T>> AsCollectionData<T>(
            [NotNull] this IObservable<ChangedListData<T>> previous)
        {
            return previous.Select(x => new ChangedCollectionData<T>(
                new ListChangeAdapter<T>(x.Change),
                x.ReachedState))
                .NotNull();
        }

        [NotNull]
        public static IObservable<IChange<ListOperation<T>>> AsChanges<T>(
            [NotNull] this IObservable<ChangedListData<T>> previous)
        {
            return previous.Select(x => x.Change).NotNull();
        }

        [NotNull]
        public static IObservable<IChange<CollectionOperation<T>>> AsChanges<T>(
            [NotNull] this IObservable<ChangedCollectionData<T>> previous)
        {
            return previous.Select(x => x.Change).NotNull();
        }
    }
}