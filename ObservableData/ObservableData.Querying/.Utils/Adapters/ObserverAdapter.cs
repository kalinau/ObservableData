using System;
using JetBrains.Annotations;

namespace ObservableData.Querying.Utils.Adapters
{
    public abstract class ObserverAdapter<T, TAdaptee> : IObserver<T>
    {
        [NotNull] private readonly IObserver<TAdaptee> _adaptee;

        protected ObserverAdapter([NotNull] IObserver<TAdaptee> adaptee)
        {
            _adaptee = adaptee;
        }

        [NotNull]
        public IObserver<TAdaptee> Adaptee => _adaptee;

        public void OnCompleted() => _adaptee.OnCompleted();

        public void OnError(Exception error) => _adaptee.OnError(error);

        public abstract void OnNext(T value);
    }

    public abstract class ObserverAdapter<T> : ObserverAdapter<T, T>
    {
        protected ObserverAdapter([NotNull] IObserver<T> adaptee) : base(adaptee)
        {
        }
    }
}