﻿using System;
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
        public static IObservable<IChange<CollectionOperation<T>>> ForSelectConstant<TPrevious, T>(
            [NotNull] this IObservable<IChange<CollectionOperation<TPrevious>>> previous,
            [NotNull] Func<TPrevious, T> func)
        {
            return Observable.Create<IChange<CollectionOperation<T>>>(o =>
            {
                if (o == null) return Disposable.Empty;

                var adapter = new SelectConstant.CollectionChangesObserver<TPrevious, T>(o, func);
                return previous.Subscribe(adapter);
            }).NotNull();
        }

        [NotNull]
        public static IObservable<CollectionChangePlusState<T>> ForSelectConstant<TPrevious, T>(
            [NotNull] this IObservable<CollectionChangePlusState<TPrevious>> previous,
            [NotNull] Func<TPrevious, T> func)
        {
            return Observable.Create<CollectionChangePlusState<T>>(o =>
            {
                if (o == null) return Disposable.Empty;

                var adapter = new SelectConstant.CollectionDataObserver<TPrevious, T>(o, func);
                return previous.Subscribe(adapter);
            }).NotNull();
        }

        [NotNull]
        public static IObservable<IChange<ListOperation<T>>> ForSelectConstant<TPrevious, T>(
            [NotNull] this IObservable<IChange<ListOperation<TPrevious>>> previous,
            [NotNull] Func<TPrevious, T> func)
        {
            return Observable.Create<IChange<ListOperation<T>>>(o =>
            {
                if (o == null) return Disposable.Empty;

                var adapter = new SelectConstant.ListChangesObserver<TPrevious, T>(o, func);
                return previous.Subscribe(adapter);
            }).NotNull();
        }

        [NotNull]
        public static IObservable<ListChangePlusState<T>> ForSelectConstant<TPrevious, T>(
            [NotNull] this IObservable<ListChangePlusState<TPrevious>> previous,
            [NotNull] Func<TPrevious, T> func)
        {
            return Observable.Create<ListChangePlusState<T>>(o =>
            {
                if (o == null) return Disposable.Empty;

                var adapter = new SelectConstant.ListDataObserver<TPrevious, T>(o, func);
                return previous.Subscribe(adapter);
            }).NotNull();
        }

        [NotNull]
        public static IObservable<CollectionChangePlusState<T>> ForSelectImmutable<TPrevious, T>(
            [NotNull] this IObservable<IChange<CollectionOperation<TPrevious>>> previous,
            [NotNull] Func<TPrevious, T> func)
        {
            return Observable.Create<CollectionChangePlusState<T>>(o =>
            {
                if (o == null) return Disposable.Empty;

                var adapter = new SelectImmutable.CollectionChangesObserver<TPrevious, T>(o, func);
                return previous.Subscribe(adapter);
            }).NotNull();
        }

        [NotNull]
        public static IObservable<CollectionChangePlusState<T>> ForSelectImmutable<TPrevious, T>(
            [NotNull] this IObservable<CollectionChangePlusState<TPrevious>> previous,
            [NotNull] Func<TPrevious, T> func)
        {
            return previous.Select(x => x.Change).NotNull().ForSelectImmutable(func);
        }

        [NotNull]
        public static IObservable<IChange<ListOperation<T>>> ForSelectImmutable<TPrevious, T>(
            [NotNull] this IObservable<IChange<ListOperation<TPrevious>>> previous,
            [NotNull] Func<TPrevious, T> func)
        {
            return Observable.Create<IChange<ListOperation<T>>>(o =>
            {
                if (o == null) return Disposable.Empty;

                var adapter = new SelectImmutable.ListChangesObserver<TPrevious, T>(o, func);
                return previous.Subscribe(adapter);
            }).NotNull();
        }

        [NotNull]
        public static IObservable<ListChangePlusState<T>> ForSelectImmutable<TPrevious, T>(
            [NotNull] this IObservable<ListChangePlusState<TPrevious>> previous,
            [NotNull] Func<TPrevious, T> func)
        {
            return Observable.Create<ListChangePlusState<T>>(o =>
            {
                if (o == null) return Disposable.Empty;

                var adapter = new SelectImmutable.ListDataObserver<TPrevious, T>(o, func);
                return previous.Subscribe(adapter);
            }).NotNull();
        }
    }
}