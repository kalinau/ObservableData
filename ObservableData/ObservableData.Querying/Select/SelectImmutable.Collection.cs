using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using ObservableData.Querying.Utils.Adapters;

namespace ObservableData.Querying.Select
{
    internal static partial class SelectImmutable
    {
        public sealed class CollectionChangesObserver<T, TAdaptee> : ObserverAdapter<IBatch<GeneralChange<T>>,
            GeneralChangesPlusState<TAdaptee>>
        {
            [NotNull] readonly SelectState<T, TAdaptee> _state = new SelectState<T, TAdaptee>();
            [NotNull] private readonly Func<T, TAdaptee> _func;

            public CollectionChangesObserver(
                [NotNull] IObserver<GeneralChangesPlusState<TAdaptee>> adaptee,
                [NotNull] Func<T, TAdaptee> func)
                : base(adaptee)
            {
                _func = func;
            }

            public override void OnNext(IBatch<GeneralChange<T>> value)
            {
                if (value == null) return;

                Dictionary<T, TAdaptee> removedOnChange = null;

                foreach (var update in value.GetIterations())
                {
                    switch (update.Type)
                    {
                        case GeneralChangeType.Add:
                            _state.OnAdd(update.Item, _func, removedOnChange);
                            break;

                        case GeneralChangeType.Remove:
                            _state.OnRemove(update.Item, ref removedOnChange);
                            break;

                        case GeneralChangeType.Clear:
                            _state.Clear();
                            break;

                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                var adapter = new CollectionChanges<T, TAdaptee>(value, _state, removedOnChange);
                var result = new GeneralChangesPlusState<TAdaptee>(adapter, _state);
                this.Adaptee.OnNext(result);
            }
        }

        private sealed class CollectionChanges<T, TAdaptee> : ThreadSensitiveChange<GeneralChange<TAdaptee>>
        {
            [NotNull] private readonly IBatch<GeneralChange<T>> _adaptee;
            [NotNull] private readonly SelectState<T, TAdaptee> _state;
            [CanBeNull] private readonly Dictionary<T, TAdaptee> _removedOnChange;

            public CollectionChanges(
                [NotNull] IBatch<GeneralChange<T>> adaptee,
                [NotNull] SelectState<T, TAdaptee> state,
                [CanBeNull] Dictionary<T, TAdaptee> removedOnChange)
            {
                _adaptee = adaptee;
                _state = state;
                _removedOnChange = removedOnChange;
            }

            protected override IEnumerable<GeneralChange<TAdaptee>> Enumerate()
            {
                foreach (var update in _adaptee.GetIterations())
                {
                    switch (update.Type)
                    {
                        case GeneralChangeType.Add:
                            var added = _state.Get(update.Item, _removedOnChange);
                            yield return GeneralChange<TAdaptee>.OnAdd(added);
                            break;

                        case GeneralChangeType.Remove:
                            var removed = _state.Get(update.Item, _removedOnChange);
                            yield return GeneralChange<TAdaptee>.OnRemove(removed);
                            break;

                        case GeneralChangeType.Clear:
                            yield return GeneralChange<TAdaptee>.OnClear();
                            break;

                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
        }
    }
}