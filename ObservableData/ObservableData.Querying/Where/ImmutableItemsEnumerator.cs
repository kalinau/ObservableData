using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using ObservableData.Querying.Utils;

namespace ObservableData.Querying.Where
{
    internal class ImmutableItemsEnumerator<T> :
        ICollectionChangeEnumerator<T>,
        ICollectionChangeMap<T>
    {
        [NotNull] private readonly CollectionState<T> _state;
        [NotNull] private readonly Func<T, bool> _criterion;

        public ImmutableItemsEnumerator([NotNull] Func<T, bool> criterion)
        {
            _criterion = criterion;
            _state = new CollectionState<T>(_criterion);
        }

        void ICollectionChangeEnumerator<T>.OnStateChanged(IReadOnlyCollection<T> state) =>
            _state.ChangeState(state);

        void ICollectionChangeEnumerator<T>.OnClear() =>
            _state.Clear();

        void ICollectionChangeEnumerator<T>.OnAdd(T item)
        {
            if (_criterion(item))
            {
                _state.IncreaseCount();
            }
        }

        void ICollectionChangeEnumerator<T>.OnRemove(T item)
        {
            if (_criterion(item))
            {
                _state.DecreaseCount();
            }
        }

        void ICollectionChangeMap<T>.RouteStateChanged(
            ICollectionChangeEnumerator<T> enumerator,
            IReadOnlyCollection<T> state) =>
            enumerator.OnStateChanged(_state);

        void ICollectionChangeMap<T>.RouteClear(ICollectionChangeEnumerator<T> enumerator) =>
            enumerator.OnClear();

        void ICollectionChangeMap<T>.RouteAdd(ICollectionChangeEnumerator<T> enumerator, T item) =>
            enumerator.OnAdd(item);

        void ICollectionChangeMap<T>.RouteRemove(ICollectionChangeEnumerator<T> enumerator, T item) =>
            enumerator.OnRemove(item);
    }
}