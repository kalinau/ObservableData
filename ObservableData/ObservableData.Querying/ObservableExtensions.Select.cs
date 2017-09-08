using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using JetBrains.Annotations;
using ObservableData.Querying.Select;
using ObservableData.Querying.Utils;

namespace ObservableData.Querying
{
    public static partial class ObservableExtensions
    {
        [NotNull]
        public static IObservableCollectionQuery<TOut> SelectConstantFromItems<TIn, TOut>(
            [NotNull] this IObservable<ICollectionChange<TIn>> source,
            [NotNull] Func<TIn, TOut> selector)
        {

            return ObservableCollectionQuery.Create<TOut>(o =>
            {
                if (o == null) return Disposable.Empty;

                var adapter = new Select.Constant.CollectionObserver<TIn, TOut>(o, selector);
                return source.Subscribe(adapter);
            }).NotNull();
        }

        [NotNull]
        public static IObservableCollectionQuery<TOut> SelectConstantFromItems<TIn, TOut>(
            [NotNull] this IObservableCollectionQuery<TIn> source,
            [NotNull] Func<TIn, TOut> selector)
        {
            return ObservableCollectionQuery.Create<TOut>(o =>
            {
                if (o == null) return Disposable.Empty;

                var adapter = new Select.Constant.CollectionObserver<TIn, TOut>(o, selector);
                return source.Subscribe(adapter);
            }).NotNull();
        }

        [NotNull]
        public static IObservable<IListChange<TOut>> SelectConstantFromItems<TIn, TOut>(
            [NotNull] this IObservable<IListChange<TIn>> source,
            [NotNull] Func<TIn, TOut> selector)
        {
            return Observable.Create<IListChange<TOut>>(o =>
            {
                if (o == null) return Disposable.Empty;

                var adapter = new SelectConstant.ListObserver<TIn, TOut>(o, selector);
                return source.Subscribe(adapter);
            }).NotNull();
        }

        [NotNull]
        public static IObservable<ICollectionChange<TOut>> SelectFromItems<TIn, TOut>(
            [NotNull] this IObservable<ICollectionChange<TIn>> source,
            [NotNull] Func<TIn, TOut> selector)
        {
            return Observable.Create<ICollectionChange<TOut>>(
                subscribe: o =>
                {
                    if (o == null) return Disposable.Empty;

                    var adapter = new SelectImmutable.CollectionObserver<TIn, TOut>(o, selector);
                    return source.Subscribe(adapter);
                }).NotNull();
        }

        [NotNull]
        public static IObservable<IListChange<TOut>> SelectFromItems<TIn, TOut>(
            [NotNull] this IObservable<IListChange<TIn>> source,
            [NotNull] Func<TIn, TOut> selector)
        {
            return Observable.Create<IListChange<TOut>>(o =>
            {
                if (o == null) return Disposable.Empty;

                var adapter = new SelectImmutable.ListObserver<TIn,TOut>(o, selector);
                return source.Subscribe(adapter);
            }).NotNull();
        }
    }
}