using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using ObservableData.Querying.Utils;

namespace ObservableData.Querying.Select.Constant
{
    public class CollectionStateStateObserver<TIn, TOut> : IQueryStateObserver<ICollectionChange<TIn>, IReadOnlyCollection<TIn>>
    {
        private sealed class State : IReadOnlyCollection<TOut>
        {
            [NotNull] private readonly IReadOnlyCollection<TIn> _source;
            [NotNull] private readonly Func<TIn, TOut> _selector;

            public State([NotNull] IReadOnlyCollection<TIn> source, [NotNull] Func<TIn, TOut> selector)
            {
                _source = source;
                _selector = selector;
            }

            public int Count => _source.Count;

            public IEnumerator<TOut> GetEnumerator() => _source.Select(_selector).GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
        }

        private sealed class Map : ICollectionChangeMap<TIn, TOut>
        {
            [NotNull] private readonly Func<TIn, TOut> _selector;

            public Map([NotNull] Func<TIn, TOut> selector)
            {
                _selector = selector;
            }

            void ICollectionChangeMap<TIn, TOut>.RouteClear(ICollectionChangeEnumerator<TOut> enumerator) =>
                enumerator.OnClear();

            void ICollectionChangeMap<TIn, TOut>.RouteAdd(ICollectionChangeEnumerator<TOut> enumerator, TIn item) =>
                enumerator.OnAdd(_selector(item));

            void ICollectionChangeMap<TIn, TOut>.RouteRemove(ICollectionChangeEnumerator<TOut> enumerator, TIn item) =>
                enumerator.OnRemove(_selector(item));
        }

        [NotNull] private readonly IQueryStateObserver<ICollectionChange<TOut>, IReadOnlyCollection<TOut>> _adaptee;
        [NotNull] private readonly Func<TIn, TOut> _selector;
        [CanBeNull] private readonly ReassignableCollectionChange<TIn, TOut> _change;

        public CollectionStateStateObserver(
            [NotNull] IQueryStateObserver<ICollectionChange<TOut>, IReadOnlyCollection<TOut>> adaptee,
            [NotNull] Func<TIn, TOut> selector)
        {
            _adaptee = adaptee;
            _selector = selector;
            _change = new ReassignableCollectionChange<TIn, TOut>(new Map(selector));
        }

        void IObserver<ICollectionChange<TIn>>.OnCompleted() => _adaptee.OnCompleted();

        void IObserver<ICollectionChange<TIn>>.OnError(Exception error) => _adaptee.OnError(error);

        void IObserver<ICollectionChange<TIn>>.OnNext(ICollectionChange<TIn> value)
        {
            if (value == null) return;

            var change = this.GetChange();
            change.Assign(value);
            _adaptee.OnNext(_change);
            change.Invalidate();
        }

        public void OnStart(IReadOnlyCollection<TIn> state)
        {
            _adaptee.OnStart(state == null ? null : new State(state, _selector));
        }

        [NotNull]
        private ReassignableCollectionChange<TIn, TOut> GetChange()
        {
            if (_change == null) throw new ArgumentOutOfRangeException();
            return _change;
        }
    }
}