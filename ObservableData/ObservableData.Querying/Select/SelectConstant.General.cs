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
        public sealed class GeneralChangesObserver<TIn, TOut> : 
            CollectionChangesObserverAdapter<TIn, TOut>
        {
            [NotNull] private readonly Func<TIn, TOut> _selector;

            public GeneralChangesObserver(
                [NotNull] IObserver<IBatch<GeneralChange<TOut>>> adaptee,
                [NotNull] Func<TIn, TOut> selector)
                : base(adaptee)
            {
                _selector = selector;
            }

            public override void OnNext(IBatch<GeneralChange<TIn>> value)
            {
                if (value == null) return;

                var adapter = new GeneralChanges<TIn, TOut>(value, _selector);
                this.Adaptee.OnNext(adapter);
            }
        }

        public sealed class GeneralChangesPlusStateObserver<TIn, TOut> : CollectionDataObserverAdapter<TIn, TOut>
        {
            [NotNull] private readonly Func<TIn, TOut> _selector;

            public GeneralChangesPlusStateObserver(
                [NotNull] IObserver<GeneralChangesPlusState<TOut>> adaptee,
                [NotNull] Func<TIn, TOut> selector)
                : base(adaptee)
            {
                _selector = selector;
            }

            public override void OnNext(GeneralChangesPlusState<TIn> value)
            {
                var change = new GeneralChanges<TIn, TOut>(value.Changes, _selector);
                var state = new CollectionAdapter<TIn, TOut>(value.ReachedState, _selector);
                this.Adaptee.OnNext(new GeneralChangesPlusState<TOut>(change, state));
            }
        }

        private sealed class GeneralChanges<TIn, TOut> : IBatch<GeneralChange<TOut>>
        {
            [NotNull] private readonly IBatch<GeneralChange<TIn>> _adaptee;
            [NotNull] private readonly Func<TIn, TOut> _selector;

            public GeneralChanges(
                [NotNull] IBatch<GeneralChange<TIn>> adaptee,
                [NotNull] Func<TIn, TOut> selector)
            {
                _adaptee = adaptee;
                _selector = selector;
            }

            public IEnumerable<GeneralChange<TOut>> GetPeaces()
            {
                foreach (var u in _adaptee.GetPeaces())
                {
                    switch (u.Type)
                    {
                        case GeneralChangeType.Add:
                            yield return GeneralChange<TOut>.OnAdd(_selector(u.Item));
                            break;

                        case GeneralChangeType.Remove:
                            yield return GeneralChange<TOut>.OnRemove(_selector(u.Item));
                            break;

                        case GeneralChangeType.Clear:
                            yield return GeneralChange<TOut>.OnClear();
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

        private sealed class CollectionAdapter<TIn, TOut> : IReadOnlyCollection<TOut>
        {
            [NotNull] private readonly IReadOnlyCollection<TIn> _source;
            [NotNull] private readonly Func<TIn, TOut> _selector;

            public CollectionAdapter([NotNull] IReadOnlyCollection<TIn> source, [NotNull] Func<TIn, TOut> selector)
            {
                _source = source;
                _selector = selector;
            }

            public int Count => _source.Count;

            public IEnumerator<TOut> GetEnumerator()
            {
                return _source.Select(_selector).GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
        }
    }
}