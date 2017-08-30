using System;
using JetBrains.Annotations;

namespace ObservableData.Querying.Criterions
{
    internal static class All
    {
        public sealed class GeneralChangesObserver<TSum> :
            IObserver<IBatch<GeneralChange<TSum>>>
        {
            [NotNull] private readonly IObserver<bool> _adaptee;
            [NotNull] private readonly Func<TSum, bool> _criterion;

            private int _count;
            private int _satisfyCount;

            public GeneralChangesObserver(
                [NotNull] IObserver<bool> adaptee, 
                [NotNull] Func<TSum, bool> criterion)
            {
                _adaptee = adaptee;
                _criterion = criterion;
            }

            public void OnNext(IBatch<GeneralChange<TSum>> change)
            {
                if (change == null) return;

                foreach (var peace in change.GetPeaces())
                {
                    switch (peace.Type)
                    {
                        case GeneralChangeType.Add:
                            _count++;
                            if (_criterion(peace.Item))
                            {
                                _satisfyCount++;
                            }
                            break;

                        case GeneralChangeType.Remove:
                            _count--;
                            if (_criterion(peace.Item))
                            {
                                _satisfyCount--;
                            }
                            break;

                        case GeneralChangeType.Clear:
                            _count = _satisfyCount = 0;
                            break;

                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                _adaptee.OnNext(_satisfyCount == _count);
            }

            public void OnCompleted() => _adaptee.OnCompleted();

            public void OnError(Exception error) => _adaptee.OnError(error);
        }
    }
}