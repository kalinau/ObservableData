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
        public sealed class GeneralChangesObserver<T, TAdaptee> : CollectionChangesObserverAdapter<T, TAdaptee>
        {
            [NotNull] private readonly Func<T, TAdaptee> _selector;

            public GeneralChangesObserver(
                [NotNull] IObserver<IBatch<GeneralChange<TAdaptee>>> adaptee,
                [NotNull] Func<T, TAdaptee> selector)
                : base(adaptee)
            {
                _selector = selector;
            }

            public override void OnNext(IBatch<GeneralChange<T>> value)
            {
                if (value == null) return;

                var adapter = new GeneralChanges<T, TAdaptee>(value, _selector);
                this.Adaptee.OnNext(adapter);
            }
        }

        public sealed class GeneralChangesPlusStateObserver<T, TAdaptee> : CollectionDataObserverAdapter<T, TAdaptee>
        {
            [NotNull] private readonly Func<T, TAdaptee> _selector;

            public GeneralChangesPlusStateObserver(
                [NotNull] IObserver<GeneralChangesPlusState<TAdaptee>> adaptee,
                [NotNull] Func<T, TAdaptee> selector)
                : base(adaptee)
            {
                _selector = selector;
            }

            public override void OnNext(GeneralChangesPlusState<T> value)
            {
                var change = new GeneralChanges<T, TAdaptee>(value.Changes, _selector);
                var state = new CollectionAdapter<T, TAdaptee>(value.ReachedState, _selector);
                this.Adaptee.OnNext(new GeneralChangesPlusState<TAdaptee>(change, state));
            }
        }

        private sealed class GeneralChanges<T, TAdaptee> : IBatch<GeneralChange<TAdaptee>>
        {
            [NotNull] private readonly IBatch<GeneralChange<T>> _adaptee;
            [NotNull] private readonly Func<T, TAdaptee> _selector;

            public GeneralChanges(
                [NotNull] IBatch<GeneralChange<T>> adaptee,
                [NotNull] Func<T, TAdaptee> selector)
            {
                _adaptee = adaptee;
                _selector = selector;
            }

            public IEnumerable<GeneralChange<TAdaptee>> GetIterations()
            {
                foreach (var u in _adaptee.GetIterations())
                {
                    switch (u.Type)
                    {
                        case GeneralChangeType.Add:
                            yield return GeneralChange<TAdaptee>.OnAdd(_selector(u.Item));
                            break;

                        case GeneralChangeType.Remove:
                            yield return GeneralChange<TAdaptee>.OnRemove(_selector(u.Item));
                            break;

                        case GeneralChangeType.Clear:
                            yield return GeneralChange<TAdaptee>.OnClear();
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