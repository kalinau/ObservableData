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
        public static IObservable<ICollectionChange<TOut>> SelectFromItems<TIn, TOut>(
            [NotNull] this IObservable<ICollectionChange<TIn>> source,
            [NotNull] Func<TIn, TOut> selector)
        {
            return Observable.Create<ICollectionChange<TOut>>(o =>
            {
                if (o == null) return Disposable.Empty;

                var adapter = new SelectImmutable.CollectionObserver<TIn, TOut>(o, selector);
                return source.Subscribe(adapter);
            }).NotNull();
        }

        //[NotNull]
        //public static IObservable<GeneralChangesPlusState<T>> SelectFromItems<TPrevious, T>(
        //    [NotNull] this IObservable<IBatch<GeneralChange<TPrevious>>> previous,
        //    [NotNull] Func<TPrevious, T> func)
        //{
        //    return Observable.Create<GeneralChangesPlusState<T>>(o =>
        //    {
        //        if (o == null) return Disposable.Empty;

        //        var adapter = new SelectImmutable.CollectionObserver<TPrevious, T>(o, func);
        //        return previous.Subscribe(adapter);
        //    }).NotNull();
        //}

        //[NotNull]
        //public static IObservable<GeneralChangesPlusState<T>> SelectFromItems<TPrevious, T>(
        //    [NotNull] this IObservable<GeneralChangesPlusState<TPrevious>> previous,
        //    [NotNull] Func<TPrevious, T> func)
        //{
        //    return previous.Select(x => x.Changes).NotNull().SelectFromItems(func);
        //}

        //[NotNull]
        //public static IObservable<IBatch<IndexedChange<T>>> SelectFromItems<TPrevious, T>(
        //    [NotNull] this IObservable<IBatch<IndexedChange<TPrevious>>> previous,
        //    [NotNull] Func<TPrevious, T> func)
        //{
        //    return Observable.Create<IBatch<IndexedChange<T>>>(o =>
        //    {
        //        if (o == null) return Disposable.Empty;

        //        var adapter = new SelectImmutable.ListChangesObserver<TPrevious, T>(o, func);
        //        return previous.Subscribe(adapter);
        //    }).NotNull();
        //}

        //[NotNull]
        //public static IObservable<IndexedChangesPlusState<T>> SelectFromItems<TPrevious, T>(
        //    [NotNull] this IObservable<IndexedChangesPlusState<TPrevious>> previous,
        //    [NotNull] Func<TPrevious, T> func)
        //{
        //    return Observable.Create<IndexedChangesPlusState<T>>(o =>
        //    {
        //        if (o == null) return Disposable.Empty;

        //        var adapter = new SelectImmutable.ListDataObserver<TPrevious, T>(o, func);
        //        return previous.Subscribe(adapter);
        //    }).NotNull();
        //}
    }
}