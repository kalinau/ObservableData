using System;
using JetBrains.Annotations;

namespace ObservableData.Querying.Math
{
    internal static class Count
    {
        public sealed class GeneralChangesObserver<T> :
            IObserver<IBatch<GeneralChange<T>>>
        {
            [NotNull] private readonly IObserver<int> _adaptee;

            private int _count;

            public GeneralChangesObserver(
                [NotNull] IObserver<int> adaptee)
            {
                _adaptee = adaptee;
            }

            public void OnNext(IBatch<GeneralChange<T>> change)
            {
                if (change == null) return;

                foreach (var peace in change.GetPeaces())
                {
                    switch (peace.Type)
                    {
                        case GeneralChangeType.Add:
                            _count++;
                            break;

                        case GeneralChangeType.Remove:
                            _count--;
                            break;

                        case GeneralChangeType.Clear:
                            _count = 0;
                            break;

                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                _adaptee.OnNext(_count);
            }

            public void OnCompleted() => _adaptee.OnCompleted();

            public void OnError(Exception error) => _adaptee.OnError(error);
        }
    }
}
