using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace ObservableData.Querying.Math
{
    internal static class Sum
    {
        public sealed class Observer<T> : 
            IObserver<ICollectionChange<T>>, 
            ITrickyEnumerator<GeneralChange<T>>
        {
            [NotNull] private readonly IObserver<T> _adaptee;
            [NotNull] private readonly Func<T, T, T> _plus;
            [NotNull] private readonly Func<T, T, T> _minus;
            [NotNull] private readonly T _zero;

            private IReadOnlyCollection<T> _currentState;
            private T _currentSum;

            public Observer(
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

                if (!this.TryChangeState(change.State))
                {
                    change.Enumerate(this);
                }

                _adaptee.OnNext(_currentSum);
            }

            bool ITrickyEnumerator<GeneralChange<T>>.OnNext(GeneralChange<T> item)
            {
                switch (item.Type)
                {
                    case GeneralChangeType.Add:
                        _currentSum = _plus(_currentSum, item.Item);
                        break;

                    case GeneralChangeType.Remove:
                        _currentSum = _minus(_currentSum, item.Item);
                        break;

                    case GeneralChangeType.Clear:
                        _currentSum = _zero;
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
                return true;
            }

            private bool TryChangeState(IReadOnlyCollection<T> state)
            {
                if (!ReferenceEquals(_currentState, state))
                {
                    _currentState = state;
                    _currentSum = _zero;
                    if (state != null)
                    {
                        foreach (var item in state)
                        {
                            _currentSum = _plus(_currentSum, item);
                        }
                    }
                    return true;
                }
                return false;
            }

        }
    }
}
