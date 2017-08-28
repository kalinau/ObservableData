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
            IBatch<IndexedChange<T>>,
            IBatch<GeneralChange<T>>
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

            IEnumerable<IndexedChange<T>> IBatch<IndexedChange<T>>.GetIterations()
            {
                int i = 0;
                foreach (var item in this.GetItems())
                {
                    yield return IndexedChange<T>.OnAdd(item, i++);
                }
            }

            IEnumerable<GeneralChange<T>> IBatch<GeneralChange<T>>.GetIterations()
            {
                foreach (var item in this.GetItems())
                {
                    yield return GeneralChange<T>.OnAdd(item);
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
        public static IObservable<IBatch<IndexedChange<T>>> StartWith<T>(
            [NotNull] this IObservable<IBatch<IndexedChange<T>>> observable,
            [NotNull] IReadOnlyCollection<T> state)
        {
            return observable.StartWith(new CurrentStateChange<T>(state)).NotNull();
        }

        [NotNull]
        public static IObservable<IBatch<GeneralChange<T>>> StartWith<T>(
            [NotNull] this IObservable<IBatch<GeneralChange<T>>> observable,
            [NotNull] IReadOnlyCollection<T> state)
        {
            return observable.StartWith(new CurrentStateChange<T>(state)).NotNull();
        }

        [NotNull]
        public static IObservable<IndexedChangesPlusState<T>> WithState<T>(
            [NotNull] this IObservable<IBatch<IndexedChange<T>>> observable,
            [NotNull] IReadOnlyList<T> state)
        {
            return observable.Select(x => new IndexedChangesPlusState<T>(x.NotNull(), state))
                .NotNull();
        }

        [NotNull]
        public static IObservable<GeneralChangesPlusState<T>> WithState<T>(
            [NotNull] this IObservable<IBatch<GeneralChange<T>>> observable,
            [NotNull] IReadOnlyCollection<T> state)
        {
            return observable.Select(x => new GeneralChangesPlusState<T>(x.NotNull(), state))
                .NotNull();
        }

    }
}