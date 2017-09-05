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
        public static IObservable<ICollectionChange<TOut>> SelectConstantFromItems<TIn, TOut>(
            [NotNull] this IObservable<ICollectionChange<TIn>> source,
            [NotNull] Func<TIn, TOut> selector)
        {
            return Observable.Create<ICollectionChange<TOut>>(o =>
            {
                if (o == null) return Disposable.Empty;

                var adapter = new SelectConstant.CollectionObserver<TIn, TOut>(o, selector);
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

                var adapter = new SelectConstant.ListObserver<TIn,TOut>(o, selector);
                return source.Subscribe(adapter);
            }).NotNull();
        }
    }
}