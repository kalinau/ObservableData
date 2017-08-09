using System;
using JetBrains.Annotations;

namespace ObservableData.Querying.Utils.Adapters
{
    public static class ObserverExtensions
    {
        private sealed class ObserverAdapter<TIn, T> : IObserver<TIn>
        {
            [NotNull] private readonly IObserver<T> _adaptee;
            [NotNull] private readonly Func<TIn, T> _handle;

            public ObserverAdapter(
                [NotNull] IObserver<T> adaptee,
                [NotNull]  Func<TIn, T> handle)
            {
                _adaptee = adaptee;
                _handle = handle;
            }

            public void OnCompleted() => _adaptee.OnCompleted();

            public void OnError(Exception error) => _adaptee.OnError(error);

            public void OnNext([NotNull] TIn value) => _adaptee.OnNext(_handle.Invoke(value));
        }

        [NotNull]
        public static IObserver<T> Wrap<T>(
            [NotNull] this IObserver<T> adaptee,
            [NotNull] Func<T, T> selectUpdate)
        {
            return new ObserverAdapter<T, T>(adaptee, selectUpdate);
        }

        [NotNull]
        public static IObserver<TIn> Wrap<TIn, T>(
            [NotNull] this IObserver<T> adaptee,
            [NotNull] Func<TIn, T> selectUpdate,
            [CanBeNull] TIn inExample)
        {
            return new ObserverAdapter<TIn, T>(adaptee, selectUpdate);
        }
    }
}