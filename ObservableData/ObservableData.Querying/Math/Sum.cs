using System;
using JetBrains.Annotations;

namespace ObservableData.Querying.Math
{
    internal static class Sum
    {
        public sealed class GeneralChangesObserver<TSum> :
            IObserver<IBatch<GeneralChange<TSum>>>
        {
            [NotNull] private readonly IObserver<TSum> _adaptee;
            [NotNull] private readonly Func<TSum, TSum, TSum> _plus;
            [NotNull] private readonly Func<TSum, TSum, TSum> _minus;
            [NotNull] private readonly TSum _zero;

            private TSum _currentSum;

            public GeneralChangesObserver(
                [NotNull] IObserver<TSum> adaptee,
                [NotNull] Func<TSum,TSum, TSum> plus,
                [NotNull] Func<TSum, TSum, TSum> minus,
                [NotNull] TSum zero)
            {
                _adaptee = adaptee;
                _plus = plus;
                _minus = minus;
                _zero = zero;
            }

            public void OnNext(IBatch<GeneralChange<TSum>> change)
            {
                if (change == null) return;

                foreach (var peace in change.GetPeaces())
                {
                    switch (peace.Type)
                    {
                        case GeneralChangeType.Add:
                            _currentSum = _plus(_currentSum, peace.Item);
                            break;

                        case GeneralChangeType.Remove:
                            _currentSum = _minus(_currentSum, peace.Item);
                            break;

                        case GeneralChangeType.Clear:
                            _currentSum = _zero;
                            break;

                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                _adaptee.OnNext(_currentSum);
            }

            public void OnCompleted() => _adaptee.OnCompleted();

            public void OnError(Exception error) => _adaptee.OnError(error);
        }
    }
}
