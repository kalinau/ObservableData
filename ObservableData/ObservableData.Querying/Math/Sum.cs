using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace ObservableData.Querying.Math
{
    internal static class Sum
    {
        public sealed class StateStateObserver<T> :
            IQueryStateObserver<,>,
            ICollectionChangeEnumerator<T>
        {
            [NotNull] private readonly IObserver<T> _adaptee;
            [NotNull] private readonly Func<T, T, T> _plus;
            [NotNull] private readonly Func<T, T, T> _minus;
            [NotNull] private readonly T _zero;

            private T _currentSum;

            public StateStateObserver(
                [NotNull] IObserver<T> adaptee,
                [NotNull] Func<T, T, T> plus,
                [NotNull] Func<T, T, T> minus,
                [NotNull] T zero)
            {
                _adaptee = adaptee;
                _plus = plus;
                _minus = minus;
                _zero = zero;
            }

            void IObserver<ICollectionChange<T>>.OnCompleted() => _adaptee.OnCompleted();

            void IObserver<ICollectionChange<T>>.OnError(Exception error) => _adaptee.OnError(error);

            void IObserver<ICollectionChange<T>>.OnNext(ICollectionChange<T> change)
            {
                if (change == null) return;

                change.Enumerate(this);
                _adaptee.OnNext(_currentSum);
            }

            void ICollectionChangeEnumerator<T>.OnStateChanged(IReadOnlyCollection<T> state)
            {
                _currentSum = _zero;
                foreach (var item in state)
                {
                    _currentSum = _plus(_currentSum, item);
                }
            }

            void ICollectionChangeEnumerator<T>.OnAdd(T item)
            {
                _currentSum = _plus(_currentSum, item);
            }

            void ICollectionChangeEnumerator<T>.OnRemove(T item)
            {
                _currentSum = _minus(_currentSum, item);
            }

            void ICollectionChangeEnumerator<T>.OnClear()
            {
                _currentSum = _zero;
            }
        }
    }
}
