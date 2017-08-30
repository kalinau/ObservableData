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
        public static IObservable<IBatch<GeneralChange<T>>> SelectConstantFromItems<TPrevious, T>(
            [NotNull] this IObservable<IBatch<GeneralChange<TPrevious>>> previous,
            [NotNull] Func<TPrevious, T> func)
        {
            return Observable.Create<IBatch<GeneralChange<T>>>(o =>
            {
                if (o == null) return Disposable.Empty;

                var adapter = new SelectConstant.GeneralChangesObserver<TPrevious, T>(o, func);
                return previous.Subscribe(adapter);
            }).NotNull();
        }

        [NotNull]
        public static IObservable<GeneralChangesPlusState<T>> SelectConstantFromItems<TPrevious, T>(
            [NotNull] this IObservable<GeneralChangesPlusState<TPrevious>> previous,
            [NotNull] Func<TPrevious, T> func)
        {
            return Observable.Create<GeneralChangesPlusState<T>>(o =>
            {
                if (o == null) return Disposable.Empty;

                var adapter = new SelectConstant.GeneralChangesPlusStateObserver<TPrevious, T>(o, func);
                return previous.Subscribe(adapter);
            }).NotNull();
        }

        [NotNull]
        public static IObservable<IBatch<IndexedChange<T>>> SelectConstantFromItems<TPrevious, T>(
            [NotNull] this IObservable<IBatch<IndexedChange<TPrevious>>> previous,
            [NotNull] Func<TPrevious, T> func)
        {
            return Observable.Create<IBatch<IndexedChange<T>>>(o =>
            {
                if (o == null) return Disposable.Empty;

                var adapter = new SelectConstant.IndexedChangesObserver<TPrevious, T>(o, func);
                return previous.Subscribe(adapter);
            }).NotNull();
        }

        [NotNull]
        public static IObservable<IndexedChangesPlusState<T>> SelectConstantFromItems<TPrevious, T>(
            [NotNull] this IObservable<IndexedChangesPlusState<TPrevious>> previous,
            [NotNull] Func<TPrevious, T> func)
        {
            return Observable.Create<IndexedChangesPlusState<T>>(o =>
            {
                if (o == null) return Disposable.Empty;

                var adapter = new SelectConstant.IndexedChangesPlusStateObserver<TPrevious, T>(o, func);
                return previous.Subscribe(adapter);
            }).NotNull();
        }

        [NotNull]
        public static IObservable<GeneralChangesPlusState<T>> SelectFromItems<TPrevious, T>(
            [NotNull] this IObservable<IBatch<GeneralChange<TPrevious>>> previous,
            [NotNull] Func<TPrevious, T> func)
        {
            return Observable.Create<GeneralChangesPlusState<T>>(o =>
            {
                if (o == null) return Disposable.Empty;

                var adapter = new SelectImmutable.CollectionChangesObserver<TPrevious, T>(o, func);
                return previous.Subscribe(adapter);
            }).NotNull();
        }

        [NotNull]
        public static IObservable<GeneralChangesPlusState<T>> SelectFromItems<TPrevious, T>(
            [NotNull] this IObservable<GeneralChangesPlusState<TPrevious>> previous,
            [NotNull] Func<TPrevious, T> func)
        {
            return previous.Select(x => x.Changes).NotNull().SelectFromItems(func);
        }

        [NotNull]
        public static IObservable<IBatch<IndexedChange<T>>> SelectFromItems<TPrevious, T>(
            [NotNull] this IObservable<IBatch<IndexedChange<TPrevious>>> previous,
            [NotNull] Func<TPrevious, T> func)
        {
            return Observable.Create<IBatch<IndexedChange<T>>>(o =>
            {
                if (o == null) return Disposable.Empty;

                var adapter = new SelectImmutable.ListChangesObserver<TPrevious, T>(o, func);
                return previous.Subscribe(adapter);
            }).NotNull();
        }

        [NotNull]
        public static IObservable<IndexedChangesPlusState<T>> SelectFromItems<TPrevious, T>(
            [NotNull] this IObservable<IndexedChangesPlusState<TPrevious>> previous,
            [NotNull] Func<TPrevious, T> func)
        {
            return Observable.Create<IndexedChangesPlusState<T>>(o =>
            {
                if (o == null) return Disposable.Empty;

                var adapter = new SelectImmutable.ListDataObserver<TPrevious, T>(o, func);
                return previous.Subscribe(adapter);
            }).NotNull();
        }
    }
}