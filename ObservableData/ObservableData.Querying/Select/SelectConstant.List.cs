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
        public sealed class ListChangesObserver<T, TAdaptee> : ListChangesObserverAdapter<T, TAdaptee>
        {
            [NotNull] private readonly Func<T, TAdaptee> _selector;

            public ListChangesObserver(
                [NotNull] IObserver<IChange<ListOperation<TAdaptee>>> adaptee,
                [NotNull] Func<T, TAdaptee> selector)
                : base(adaptee)
            {
                _selector = selector;
            }

            public override void OnNext(IChange<ListOperation<T>> value)
            {
                if (value == null) return;

                var adapter = new ListChange<T, TAdaptee>(value, _selector);
                this.Adaptee.OnNext(adapter);
            }
        }

        public sealed class ListDataObserver<T, TAdaptee> : ListDataObserverAdapter<T, TAdaptee>
        {
            [NotNull] private readonly Func<T, TAdaptee> _selector;

            public ListDataObserver(
                [NotNull] IObserver<ListChangePlusState<TAdaptee>> adaptee,
                [NotNull] Func<T, TAdaptee> selector)
                : base(adaptee)
            {
                _selector = selector;
            }

            public override void OnNext(ListChangePlusState<T> value)
            {
                var change = new ListChange<T, TAdaptee>(value.Change, _selector);
                var state = new ListAdapter<T, TAdaptee>(value.ReachedState, _selector);
                this.Adaptee.OnNext(new ListChangePlusState<TAdaptee>(change, state));
            }
        }

        private sealed class ListChange<T, TAdaptee> : IChange<ListOperation<TAdaptee>>
        {
            [NotNull] private readonly IChange<ListOperation<T>> _adaptee;
            [NotNull] private readonly Func<T, TAdaptee> _selector;

            public ListChange(
                [NotNull] IChange<ListOperation<T>> adaptee,
                [NotNull] Func<T, TAdaptee> selector)
            {
                _adaptee = adaptee;
                _selector = selector;
            }

            public IEnumerable<ListOperation<TAdaptee>> GetIterations()
            {
                foreach (var update in _adaptee.GetIterations())
                {
                    switch (update.Type)
                    {
                        case ListOperationType.Add:
                            yield return ListOperation<TAdaptee>.OnAdd(
                                _selector(update.Item),
                                update.Index);
                            break;

                        case ListOperationType.Remove:
                            yield return ListOperation<TAdaptee>.OnRemove(
                                _selector(update.Item),
                                update.Index);
                            break;

                        case ListOperationType.Move:
                            yield return ListOperation<TAdaptee>.OnMove(
                                _selector(update.Item),
                                update.Index,
                                update.OriginalIndex);
                            break;

                        case ListOperationType.Replace:
                            yield return ListOperation<TAdaptee>.OnReplace(
                                _selector(update.Item),
                                _selector(update.ChangedItem),
                                update.Index);
                            break;

                        case ListOperationType.Clear:
                            yield return ListOperation<TAdaptee>.OnClear();
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