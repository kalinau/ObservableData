using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Disposables;
using JetBrains.Annotations;
using ObservableData.Querying.Utils;
using System.Reactive.Linq;
using ObservableData.Querying.Utils.Adapters;

namespace ObservableData.Querying
{
    [PublicAPI]
    public static partial class ObservableExtensions
    {
        [NotNull]
        public static IObservable<ICollectionChange<T>> StartWithState<T>(
            [NotNull] this IObservable<ICollectionChange<T>> observable,
            [NotNull] IReadOnlyCollection<T> state)
        {
            return observable.StartWith(new CollectionStateChange<T>(state)).NotNull();
        }

        [NotNull]
        public static IObservable<IListChange<T>> StartWithState<T>(
            [NotNull] this IObservable<IListChange<T>> observable,
            [NotNull] IReadOnlyList<T> state)
        {
            return observable.StartWith(new ListStateChange<T>(state)).NotNull();
        }
    }

    public static class ObservableCollectionQuery
    {
        public static IObservableCollectionQuery<T> Create<T>(
            [NotNull] Func<IQueryObserver<ICollectionChange<T>, IReadOnlyCollection<T>>, IDisposable> subscibeQuery)
        {
            return new AnonymousQueryObservable<T>(subscribe, subscibeQuery);
        }

        //public static IQueryObservable<TResult> Create<TResult>(
        //    Func<IObserver<TResult>, IDisposable> subscribe)
        //{
        //    return new AnonymousQueryObservable<TResult>(subscribe, subscribe);
        //}
    }

    public sealed class AnonymousQueryObservable<T> : ObservableBase<T>, IQueryObservable<T>
    {
        [NotNull] private readonly Func<IObserver<T>, IDisposable> _defaultSubscribe;
        [NotNull] private readonly Func<IQueryObserver<,>, IDisposable> _querySubscribe;

        public AnonymousQueryObservable(
            [NotNull] Func<IObserver<T>, IDisposable> defaultSubscribe,
            [NotNull] Func<IQueryObserver<,>, IDisposable> querySubscribe)
        {
            _defaultSubscribe = defaultSubscribe;
            _querySubscribe = querySubscribe;
        }

        public IDisposable Subscribe(IQueryObserver<,> observer)
        {
            return _querySubscribe(observer) ?? Disposable.Empty;
        }

        protected override IDisposable SubscribeCore(IObserver<T> observer)
        {
            return _defaultSubscribe(observer) ?? Disposable.Empty;
        }
    }
}