using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using ObservableData.Querying.Utils;
using System.Reactive.Linq;

namespace ObservableData.Querying
{
    public static partial class ObservableExtensions
    {
        public class CurrentStateChange<T> :
            IChange<ListOperation<T>>,
            IChange<CollectionOperation<T>>
        {
            [NotNull] private readonly IReadOnlyCollection<T> _items;
            [CanBeNull] private IReadOnlyCollection<T> _locked;

            private readonly ThreadId _threadId;

            public CurrentStateChange([NotNull] IReadOnlyCollection<T> items)
            {
                _items = items;
                _threadId = ThreadId.FromCurrent();
            }

            public void MakeImmutable()
            {
                _threadId.CheckIsCurrent();
                if (_locked == null)
                {
                    _locked = _items.ToList();
                }
            }

            IEnumerable<ListOperation<T>> IChange<ListOperation<T>>.GetIterations()
            {
                int i = 0;
                foreach (var item in this.GetItems())
                {
                    yield return ListOperation<T>.OnAdd(item, i++);
                }
            }

            IEnumerable<CollectionOperation<T>> IChange<CollectionOperation<T>>.GetIterations()
            {
                foreach (var item in this.GetItems())
                {
                    yield return CollectionOperation<T>.OnAdd(item);
                }
            }

            [NotNull]
            private IReadOnlyCollection<T> GetItems()
            {
                if (_locked != null)
                {
                    return _locked;
                }
                _threadId.CheckIsCurrent();
                return _items;
            }
        }

        [NotNull]
        public static IObservable<IChange<ListOperation<T>>> StartWith<T>(
            [NotNull] this IObservable<IChange<ListOperation<T>>> observable,
            [NotNull] IReadOnlyCollection<T> state)
        {
            return observable.StartWith(new CurrentStateChange<T>(state)).NotNull();
        }

        [NotNull]
        public static IObservable<IChange<CollectionOperation<T>>> StartWith<T>(
            [NotNull] this IObservable<IChange<CollectionOperation<T>>> observable,
            [NotNull] IReadOnlyCollection<T> state)
        {
            return observable.StartWith(new CurrentStateChange<T>(state)).NotNull();
        }

        [NotNull]
        public static IObservable<ListChangePlusState<T>> WithState<T>(
            [NotNull] this IObservable<IChange<ListOperation<T>>> observable,
            [NotNull] IReadOnlyList<T> state)
        {
            return observable.Select(x => new ListChangePlusState<T>(x.NotNull(), state))
                .NotNull();
        }

        [NotNull]
        public static IObservable<CollectionChangePlusState<T>> WithState<T>(
            [NotNull] this IObservable<IChange<CollectionOperation<T>>> observable,
            [NotNull] IReadOnlyCollection<T> state)
        {
            return observable.Select(x => new CollectionChangePlusState<T>(x.NotNull(), state))
                .NotNull();
        }

    }
}