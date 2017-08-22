using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using ObservableData.Querying.Utils.Adapters;

namespace ObservableData.Querying.Select
{
    internal static partial class SelectConstant
    {
        public sealed class CollectionChangesObserver<T, TAdaptee> : CollectionChangesObserverAdapter<T, TAdaptee>
        {
            [NotNull] private readonly Func<T, TAdaptee> _selector;

            public CollectionChangesObserver(
                [NotNull] IObserver<IChange<CollectionOperation<TAdaptee>>> adaptee,
                [NotNull] Func<T, TAdaptee> selector)
                : base(adaptee)
            {
                _selector = selector;
            }

            public override void OnNext(IChange<CollectionOperation<T>> value)
            {
                if (value == null) return;

                var adapter = new CollectionChange<T, TAdaptee>(value, _selector);
                this.Adaptee.OnNext(adapter);
            }
        }

        public sealed class CollectionDataObserver<T, TAdaptee> : CollectionDataObserverAdapter<T, TAdaptee>
        {
            [NotNull] private readonly Func<T, TAdaptee> _selector;

            public CollectionDataObserver(
                [NotNull] IObserver<ChangedCollectionData<TAdaptee>> adaptee,
                [NotNull] Func<T, TAdaptee> selector)
                : base(adaptee)
            {
                _selector = selector;
            }

            public override void OnNext(ChangedCollectionData<T> value)
            {
                var change = new CollectionChange<T, TAdaptee>(value.Change, _selector);
                var state = new CollectionAdapter<T, TAdaptee>(value.ReachedState, _selector);
                this.Adaptee.OnNext(new ChangedCollectionData<TAdaptee>(change, state));
            }
        }


        private sealed class CollectionChange<T, TAdaptee> : IChange<CollectionOperation<TAdaptee>>
        {
            [NotNull] private readonly IChange<CollectionOperation<T>> _adaptee;
            [NotNull] private readonly Func<T, TAdaptee> _selector;

            public CollectionChange(
                [NotNull] IChange<CollectionOperation<T>> adaptee,
                [NotNull] Func<T, TAdaptee> selector)
            {
                _adaptee = adaptee;
                _selector = selector;
            }

            public IEnumerable<CollectionOperation<TAdaptee>> Iterations()
            {
                foreach (var u in _adaptee.Iterations())
                {
                    switch (u.Type)
                    {
                        case CollectionOperationType.Add:
                            yield return CollectionOperation<TAdaptee>.OnAdd(_selector(u.Item));
                            break;

                        case CollectionOperationType.Remove:
                            yield return CollectionOperation<TAdaptee>.OnRemove(_selector(u.Item));
                            break;

                        case CollectionOperationType.Clear:
                            yield return CollectionOperation<TAdaptee>.OnClear();
                            break;

                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }

            public void MakeImmutable()
            {
                _adaptee.MakeImmutable();
            }
        }

        private sealed class CollectionAdapter<T, TAdaptee> : IReadOnlyCollection<TAdaptee>
        {
            [NotNull] private readonly IReadOnlyCollection<T> _source;
            [NotNull] private readonly Func<T, TAdaptee> _selector;

            public CollectionAdapter([NotNull] IReadOnlyCollection<T> source, [NotNull] Func<T, TAdaptee> selector)
            {
                _source = source;
                _selector = selector;
            }

            public int Count => _source.Count;

            public IEnumerator<TAdaptee> GetEnumerator()
            {
                return _source.Select(_selector).GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
        }
    }
}