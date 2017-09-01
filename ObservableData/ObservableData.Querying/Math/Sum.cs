using System;
using JetBrains.Annotations;

namespace ObservableData.Querying.Math
{
    internal static class Sum
    {
        public sealed class Observer<T> : IObserver<ICollectionChange<T>>
        {
            [NotNull] private readonly IObserver<T> _adaptee;
            [NotNull] private readonly Func<T, T, T> _plus;
            [NotNull] private readonly Func<T, T, T> _minus;
            [NotNull] private readonly T _zero;

            private T _currentSum;
            
            [NotNull] private readonly Func<GeneralChange<T>, bool> _onChange;

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

                _onChange = this.OnChange;
            }

            public void OnNext(ICollectionChange<T> change)
            {
                if (change == null) return;

                var state = change.TryGetState();
                if (state != null)
                {
                    _currentSum = _zero;
                    foreach (var item in state)
                    {
                        _currentSum = _plus(_currentSum, item);
                    }
                }
                else
                {
                    var delta = change.TryGetDelta();
                    delta?.Enumerate(_onChange);
                }
                _adaptee.OnNext(_currentSum);
            }

            private bool OnChange(GeneralChange<T> delta)
            {
                switch (delta.Type)
                {
                    case GeneralChangeType.Add:
                        _currentSum = _plus(_currentSum, delta.Item);
                        break;

                    case GeneralChangeType.Remove:
                        _currentSum = _minus(_currentSum, delta.Item);
                        break;

                    case GeneralChangeType.Clear:
                        _currentSum = _zero;
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
                return true;
            }

            public void OnCompleted() => _adaptee.OnCompleted();

            public void OnError(Exception error) => _adaptee.OnError(error);
        }
    }
}
