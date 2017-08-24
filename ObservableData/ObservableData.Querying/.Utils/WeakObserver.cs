using System;
using JetBrains.Annotations;

namespace ObservableData.Querying.Utils
{
    public sealed class WeakObserver<T> : IObserver<T>, IDisposable
    {
        [CanBeNull]
        private IDisposable _strongSubscription;

        [NotNull]
        private readonly WeakReference<IObserver<T>> _observer;

        public WeakObserver(IObserver<T> underlying)
        {
            _observer = new WeakReference<IObserver<T>>(underlying);
        }

        public void OnNext(T value)
        {
            _observer.TryGetTarget(out var o);
            if (o != null)
            {
                o.OnNext(value);
            }
            else
            {
                this.Dispose();
            }
        }

        public void OnError(Exception exception)
        {
            _observer.TryGetTarget(out var o);
            o?.OnError(exception);
            this.Dispose();
        }

        public void OnCompleted()
        {
            _observer.TryGetTarget(out var o);
            o?.OnCompleted();
            this.Dispose();
        }

        public void SaveSubscription(IDisposable subscription)
        {
            _strongSubscription = subscription;
        }

        public void Dispose()
        {
            _strongSubscription?.Dispose();
            _strongSubscription = null;
        }
    }
}