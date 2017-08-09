using System;
using System.Reactive;
using System.Reactive.Linq;
using JetBrains.Annotations;

namespace ObservableData.Querying.Utils
{
    [PublicAPI]
    public static class ObservableExtensions
    {
        [NotNull]
        public static IObservable<T> AsWeakObservable<T>(this IObservable<T> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            return Observable.Create<T>(o => source.WeakSubscribe(o)).NotNull();
        }

        [NotNull]
        public static IDisposable WeakSubscribe<T>([NotNull] this IObservable<T> source, Action<T> onNext)
        {
            return source.WeakSubscribe(Observer.Create(onNext));
        }

        [NotNull]
        public static IDisposable WeakSubscribe<T>([NotNull] this IObservable<T> source, IObserver<T> o)
        {
            var observer = new WeakObserver<T>(o);
            var strongSubscription = source.Subscribe(observer);
            observer.SaveSubscription(strongSubscription);

            return new KeedAliveDisposableAdapter(observer, o);
        }

        public static void KeepAlive<T>([NotNull] this IObservable<T> source, object keepAliveObject)
        {
            source.Subscribe(_ => GC.KeepAlive(keepAliveObject));
        }
    }
}