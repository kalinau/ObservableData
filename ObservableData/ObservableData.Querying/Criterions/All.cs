using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using System.Linq;

namespace ObservableData.Querying.Criterions
{
    internal static class All
    {
        public sealed class Observer<T> : 
            IQueryObserver<,>,
            ICollectionChangeEnumerator<T>
        {
            [NotNull] private readonly IObserver<bool> _adaptee;
            [NotNull] private readonly Func<T, bool> _criterion;

            private bool? _isAll;

            private int _count;
            private int _satisfyCount;

            public Observer(
                [NotNull] IObserver<bool> adaptee,
                [NotNull] Func<T, bool> criterion)
            {
                _adaptee = adaptee;
                _criterion = criterion;
            }

            void IObserver<ICollectionChange<T>>.OnCompleted() => _adaptee.OnCompleted();

            void IObserver<ICollectionChange<T>>.OnError(Exception error) => _adaptee.OnError(error);

            void IObserver<ICollectionChange<T>>.OnNext(ICollectionChange<T> change)
            {
                if (change == null) return;

                change.Enumerate(this);

                var isAll = _count > 0 && _satisfyCount == _count;
                if (isAll != _isAll)
                {
                    _isAll = isAll;
                    _adaptee.OnNext(isAll);
                }
            }

            void ICollectionChangeEnumerator<T>.OnStateChanged(IReadOnlyCollection<T> state)
            {
                _count = state.Count;
                _satisfyCount = state.Count(_criterion);
            }

            void ICollectionChangeEnumerator<T>.OnClear()
            {
                _count = _satisfyCount = 0;
            }

            void ICollectionChangeEnumerator<T>.OnAdd(T item)
            {
                _count++;
                if (_criterion(item))
                {
                    _satisfyCount++;
                }
            }

            void ICollectionChangeEnumerator<T>.OnRemove(T item)
            {
                _count--;
                if (_criterion(item))
                {
                    _satisfyCount--;
                }
            }
        }
    }
}