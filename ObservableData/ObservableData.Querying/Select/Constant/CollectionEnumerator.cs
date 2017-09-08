using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using ObservableData.Querying.Utils;

namespace ObservableData.Querying.Select.Constant
{
    internal sealed class CollectionEnumerator<TIn, TOut> : 
        ICollectionChangeMap<TIn, TOut>,
        ICollectionChangeEnumerator<TIn>
    {
        private sealed class State : IReadOnlyCollection<TOut>
        {
            [NotNull] private IReadOnlyCollection<TIn> _source = EmptyList<TIn>.Instance;
            [NotNull] private readonly Func<TIn, TOut> _selector;

            public State([NotNull] Func<TIn, TOut> selector)
            {
                _selector = selector;
            }

            public void ChangeSource([NotNull] IReadOnlyCollection<TIn> source)
            {
                _source = source;
            }

            public int Count => _source.Count;

            public IEnumerator<TOut> GetEnumerator() => _source.Select(_selector).GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
        }

        [NotNull] private readonly Func<TIn, TOut> _selector;
        [NotNull] private readonly State _state;

        public CollectionEnumerator([NotNull] Func<TIn, TOut> selector)
        {
            _selector = selector;
            _state = new State(_selector);
        }

        void ICollectionChangeMap<TIn, TOut>.RouteStateChanged(ICollectionChangeEnumerator<TOut> enumerator, IReadOnlyCollection<TIn> state) => 
            enumerator.OnStateChanged(_state);

        void ICollectionChangeMap<TIn, TOut>.RouteClear(ICollectionChangeEnumerator<TOut> enumerator) =>
            enumerator.OnClear();

        void ICollectionChangeMap<TIn, TOut>.RouteAdd(ICollectionChangeEnumerator<TOut> enumerator, TIn item) =>
            enumerator.OnAdd(_selector(item));

        void ICollectionChangeMap<TIn, TOut>.RouteRemove(ICollectionChangeEnumerator<TOut> enumerator, TIn item) =>
            enumerator.OnRemove(_selector(item));

        void ICollectionChangeEnumerator<TIn>.OnStateChanged(IReadOnlyCollection<TIn> state) => 
            _state.ChangeSource(state);

        void ICollectionChangeEnumerator<TIn>.OnClear() { }

        void ICollectionChangeEnumerator<TIn>.OnAdd(TIn item) { }

        void ICollectionChangeEnumerator<TIn>.OnRemove(TIn item) { }
    }
}