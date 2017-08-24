using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using ObservableData.Querying.Utils.Adapters;

namespace ObservableData.Querying.Where
{
    internal static class WhereByImmutable
    {
        public sealed class CollectionChangesObserver<T> : CollectionChangesObserverAdapter<T>
        {
            [NotNull] private readonly Func<T, bool> _criterion;

            public CollectionChangesObserver(
                [NotNull] IObserver<IChange<CollectionOperation<T>>> previous,
                [NotNull] Func<T, bool> criterion) 
                : base(previous)
            {
                _criterion = criterion;
            }

            public override void OnNext(IChange<CollectionOperation<T>> value)
            {
                if (value == null) return;

                this.Adaptee.OnNext(new CollectionChange<T>(value, _criterion));
            }
        }

        public sealed class CollectionDataObserver<T> : CollectionDataObserverAdapter<T>
        {
            [NotNull] private readonly Func<T, bool> _criterion;

            public CollectionDataObserver(
                [NotNull] IObserver<CollectionChangePlusState<T>> previous,
                [NotNull] Func<T, bool> criterion)
                : base(previous)
            {
                _criterion = criterion;
            }
            public override void OnNext(CollectionChangePlusState<T> value)
            {
                var change = new CollectionChange<T>(value.Change, _criterion);
                var state = new CollectionAdapter<T>(value.ReachedState, _criterion);
                this.Adaptee.OnNext(new CollectionChangePlusState<T>(change, state));
            }
        }

        private sealed class CollectionChange<T> : IChange<CollectionOperation<T>>
        {
            [NotNull] private readonly IChange<CollectionOperation<T>> _adaptee;
            [NotNull] private readonly Func<T, bool> _criterion;

            public CollectionChange(
                [NotNull] IChange<CollectionOperation<T>> adaptee,
                [NotNull] Func<T, bool> criterion)
            {
                _adaptee = adaptee;
                _criterion = criterion;
            }
            public void MakeImmutable() => _adaptee.MakeImmutable();

            public IEnumerable<CollectionOperation<T>> GetIterations()
            {
                foreach (var update in _adaptee.GetIterations())
                {
                    if (update.Type == CollectionOperationType.Clear)
                    {
                        yield return update;
                    }
                    else if (_criterion.Invoke(update.Item))
                    {
                        yield return update;
                    }
                }
            }
        }

        private sealed class CollectionAdapter<T> : IReadOnlyCollection<T>
        {
            [NotNull] private readonly IReadOnlyCollection<T> _source;
            [NotNull] private readonly Func<T, bool> _criterion;

            public CollectionAdapter([NotNull] IReadOnlyCollection<T> source,
                [NotNull] Func<T, bool> criterion)
            {
                _source = source;
                _criterion = criterion;
            }

            public int Count => _source.Count;

            public IEnumerator<T> GetEnumerator()
            {
                return _source.Where(_criterion).GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
        }
    }
}