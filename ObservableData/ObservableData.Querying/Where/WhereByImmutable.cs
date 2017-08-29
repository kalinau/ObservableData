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
                [NotNull] IObserver<IBatch<GeneralChange<T>>> previous,
                [NotNull] Func<T, bool> criterion) 
                : base(previous)
            {
                _criterion = criterion;
            }

            public override void OnNext(IBatch<GeneralChange<T>> value)
            {
                if (value == null) return;

                this.Adaptee.OnNext(new CollectionChanges<T>(value, _criterion));
            }
        }

        public sealed class CollectionDataObserver<T> : CollectionDataObserverAdapter<T>
        {
            [NotNull] private readonly Func<T, bool> _criterion;

            public CollectionDataObserver(
                [NotNull] IObserver<GeneralChangesPlusState<T>> previous,
                [NotNull] Func<T, bool> criterion)
                : base(previous)
            {
                _criterion = criterion;
            }
            public override void OnNext(GeneralChangesPlusState<T> value)
            {
                var change = new CollectionChanges<T>(value.Changes, _criterion);
                var state = new CollectionAdapter<T>(value.ReachedState, _criterion);
                this.Adaptee.OnNext(new GeneralChangesPlusState<T>(change, state));
            }
        }

        private sealed class CollectionChanges<T> : IBatch<GeneralChange<T>>
        {
            [NotNull] private readonly IBatch<GeneralChange<T>> _adaptee;
            [NotNull] private readonly Func<T, bool> _criterion;

            public CollectionChanges(
                [NotNull] IBatch<GeneralChange<T>> adaptee,
                [NotNull] Func<T, bool> criterion)
            {
                _adaptee = adaptee;
                _criterion = criterion;
            }

            public IEnumerable<GeneralChange<T>> GetPeaces()
            {
                foreach (var update in _adaptee.GetPeaces())
                {
                    if (update.Type == GeneralChangeType.Clear)
                    {
                        yield return update;
                    }
                    else if (_criterion.Invoke(update.Item))
                    {
                        yield return update;
                    }
                }
            }

            public void MakeImmutable() => _adaptee.MakeImmutable();
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