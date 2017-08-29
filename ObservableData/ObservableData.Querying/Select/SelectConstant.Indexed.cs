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
        public sealed class IndexedChangesObserver<T, TAdaptee> : ListChangesObserverAdapter<T, TAdaptee>
        {
            [NotNull] private readonly Func<T, TAdaptee> _selector;

            public IndexedChangesObserver(
                [NotNull] IObserver<IBatch<IndexedChange<TAdaptee>>> adaptee,
                [NotNull] Func<T, TAdaptee> selector)
                : base(adaptee)
            {
                _selector = selector;
            }

            public override void OnNext(IBatch<IndexedChange<T>> value)
            {
                if (value == null) return;

                var adapter = new IndexedChanges<T, TAdaptee>(value, _selector);
                this.Adaptee.OnNext(adapter);
            }
        }

        public sealed class IndexedChangesPlusStateObserver<T, TAdaptee> : ListDataObserverAdapter<T, TAdaptee>
        {
            [NotNull] private readonly Func<T, TAdaptee> _selector;

            public IndexedChangesPlusStateObserver(
                [NotNull] IObserver<IndexedChangesPlusState<TAdaptee>> adaptee,
                [NotNull] Func<T, TAdaptee> selector)
                : base(adaptee)
            {
                _selector = selector;
            }

            public override void OnNext(IndexedChangesPlusState<T> value)
            {
                var change = new IndexedChanges<T, TAdaptee>(value.Change, _selector);
                var state = new ListAdapter<T, TAdaptee>(value.ReachedState, _selector);
                this.Adaptee.OnNext(new IndexedChangesPlusState<TAdaptee>(change, state));
            }
        }

        private sealed class IndexedChanges<T, TAdaptee> : IBatch<IndexedChange<TAdaptee>>
        {
            [NotNull] private readonly IBatch<IndexedChange<T>> _adaptee;
            [NotNull] private readonly Func<T, TAdaptee> _selector;

            public IndexedChanges(
                [NotNull] IBatch<IndexedChange<T>> adaptee,
                [NotNull] Func<T, TAdaptee> selector)
            {
                _adaptee = adaptee;
                _selector = selector;
            }

            public IEnumerable<IndexedChange<TAdaptee>> GetPeaces()
            {
                foreach (var update in _adaptee.GetPeaces())
                {
                    switch (update.Type)
                    {
                        case IndexedChangeType.Add:
                            yield return IndexedChange<TAdaptee>.OnAdd(
                                _selector(update.Item),
                                update.Index);
                            break;

                        case IndexedChangeType.Remove:
                            yield return IndexedChange<TAdaptee>.OnRemove(
                                _selector(update.Item),
                                update.Index);
                            break;

                        case IndexedChangeType.Move:
                            yield return IndexedChange<TAdaptee>.OnMove(
                                _selector(update.Item),
                                update.Index,
                                update.OriginalIndex);
                            break;

                        case IndexedChangeType.Replace:
                            yield return IndexedChange<TAdaptee>.OnReplace(
                                _selector(update.Item),
                                _selector(update.ChangedItem),
                                update.Index);
                            break;

                        case IndexedChangeType.Clear:
                            yield return IndexedChange<TAdaptee>.OnClear();
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

        private sealed class ListAdapter<T, TAdaptee> : IReadOnlyList<TAdaptee>
        {
            [NotNull] private readonly IReadOnlyList<T> _source;
            [NotNull] private readonly Func<T, TAdaptee> _selector;

            public ListAdapter([NotNull] IReadOnlyList<T> source, [NotNull] Func<T, TAdaptee> selector)
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

            public TAdaptee this[int index] => _selector(_source[index]);
        }
    }
}