using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using JetBrains.Annotations;
using ObservableData.Querying.Math;
using ObservableData.Querying.Utils;

namespace ObservableData.Querying
{
    public static partial class ObservableExtensions
    {
        [NotNull]
        public static IObservable<int> SumItems(
            [NotNull] this IObservable<ICollectionChange<int>> previous)
        {
            return Observable.Create<int>(observer =>
            {
                if (observer == null) return Disposable.Empty;

                var adapter = new Sum.StateStateObserver<int>(
                    observer,
                    (x, y) => x + y,
                    (x, y) => x - y,
                    0);
                return previous.Subscribe(adapter);
            }).NotNull();
        }

        [NotNull]
        public static IObservable<long> SumItems(
            [NotNull] this IObservable<ICollectionChange<long>> previous)
        {
            return Observable.Create<long>(observer =>
            {
                if (observer == null) return Disposable.Empty;

                var adapter = new Sum.StateStateObserver<long>(
                    observer,
                    (x, y) => x + y,
                    (x, y) => x - y,
                    0);
                return previous.Subscribe(adapter);
            }).NotNull();
        }

        [NotNull]
        public static IObservable<double> SumItems(
            [NotNull] this IObservable<ICollectionChange<double>> previous)
        {
            return Observable.Create<double>(observer =>
            {
                if (observer == null) return Disposable.Empty;

                var adapter = new Sum.StateStateObserver<double>(
                    observer,
                    (x, y) => x + y,
                    (x, y) => x - y,
                    0);
                return previous.Subscribe(adapter);
            }).NotNull();
        }

        [NotNull]
        public static IObservable<float> SumItems(
            [NotNull] this IObservable<ICollectionChange<float>> previous)
        {
            return Observable.Create<float>(observer =>
            {
                if (observer == null) return Disposable.Empty;

                var adapter = new Sum.StateStateObserver<float>(
                    observer,
                    (x, y) => x + y,
                    (x, y) => x - y,
                    0);
                return previous.Subscribe(adapter);
            }).NotNull();
        }

        [NotNull]
        public static IObservable<decimal> SumItems(
            [NotNull] this IObservable<ICollectionChange<decimal>> previous)
        {
            return Observable.Create<decimal>(observer =>
            {
                if (observer == null) return Disposable.Empty;

                var adapter = new Sum.StateStateObserver<decimal>(
                    observer,
                    (x, y) => x + y,
                    (x, y) => x - y,
                    0);
                return previous.Subscribe(adapter);
            }).NotNull();
        }
    }
}