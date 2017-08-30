using System;
using JetBrains.Annotations;

namespace ObservableData.Querying.Boolean
{
    public static class Any
    {
        public sealed class GeneralChangesObserver<TSum> :
            IObserver<IBatch<GeneralChange<TSum>>>
        {
            [NotNull] private readonly IObserver<bool> _adaptee;
            [NotNull] private readonly Func<TSum, bool> _criterion;

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
                            if (_criterion(peace.Item))
                            {
                                _satisfyCount++;
                            }
                            break;

                        case GeneralChangeType.Remove:
                            if (_criterion(peace.Item))
                            {
                                _satisfyCount--;
                            }
                            break;

                        case GeneralChangeType.Clear:
                            _satisfyCount = 0;
                            break;

                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                _adaptee.OnNext(_satisfyCount > 0);
            }

            public void OnCompleted() => _adaptee.OnCompleted();

            public void OnError(Exception error) => _adaptee.OnError(error);
        }
    }
}