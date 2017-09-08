using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using ObservableData.Querying.Utils;

namespace ObservableData.Querying.Where
{
    public class CollectionStateStateObserver<T> : IQueryStateObserver<ICollectionChange<T>, IReadOnlyCollection<T>>
    {
        internal sealed class CollectionState : IReadOnlyCollection<T>, ICollectionChangeEnumerator<T>
        {
            [NotNull] private readonly IReadOnlyCollection<T> _source = EmptyList<T>.Instance;
            [NotNull] private readonly Func<T, bool> _criterion;

            private int? _count;

            public CollectionState([NotNull] Func<T, bool> criterion)
            {
                _criterion = criterion;
            }

            int IReadOnlyCollection<T>.Count
            {
                get
                {
                    if (_count == null)
                    {
                        _count = _source.Count(_criterion);
                    }
                    return _count.Value;
                }
            }

            public IEnumerator<T> GetEnumerator()
            {
                return _source.Where(_criterion).GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

            void ICollectionChangeEnumerator<T>.OnClear()
            {
                _count = 0;
            }

            void ICollectionChangeEnumerator<T>.OnAdd(T item)
            {
                if (_count != null)
                {
                    _count++;
                }
            }

            void ICollectionChangeEnumerator<T>.OnRemove(T item)
            {
                if (_count != null)
                {
                    _count--;
                }
            }
        }

        private sealed class Map : ICollectionChangeMap<T, T>
        {
            [NotNull] private readonly Func<T, T> _selector;

            public Map([NotNull] Func<T, T> selector)
            {
                _selector = selector;
            }

            void ICollectionChangeMap<T, T>.RouteClear(ICollectionChangeEnumerator<T> enumerator) =>
                enumerator.OnClear();

            void ICollectionChangeMap<T, T>.RouteAdd(ICollectionChangeEnumerator<T> enumerator, T item) =>
                enumerator.OnAdd(_selector(item));

            void ICollectionChangeMap<T, T>.RouteRemove(ICollectionChangeEnumerator<T> enumerator, T item) =>
                enumerator.OnRemove(_selector(item));
        }

        [NotNull] private readonly IQueryStateObserver<ICollectionChange<T>, IReadOnlyCollection<T>> _adaptee;
        [NotNull] private readonly Func<T, T> _selector;
        [CanBeNull] private readonly ReassignableCollectionChange<T, T> _change;

        public CollectionStateStateObserver(
            [NotNull] IQueryStateObserver<ICollectionChange<T>, IReadOnlyCollection<T>> adaptee,
            [NotNull] Func<T, TOut> selector)
        {
            _adaptee = adaptee;
            _selector = selector;
            _change = new ReassignableCollectionChange<T, TOut>(new Map(selector));
        }

        void IObserver<ICollectionChange<T>>.OnCompleted() => _adaptee.OnCompleted();

        void IObserver<ICollectionChange<T>>.OnError(Exception error) => _adaptee.OnError(error);

        void IObserver<ICollectionChange<T>>.OnNext(ICollectionChange<T> value)
        {
            if (value == null) return;

            var change = this.GetChange();
            change.Assign(value);
            _adaptee.OnNext(_change);
            change.Invalidate();
        }

        public void OnStart(IReadOnlyCollection<T> state)
        {
            _adaptee.OnStart(state == null ? null : new State(state, _selector));
        }

        [NotNull]
        private ReassignableCollectionChange<T, TOut> GetChange()
        {
            if (_change == null) throw new ArgumentOutOfRangeException();
            return _change;
        }
    }
}