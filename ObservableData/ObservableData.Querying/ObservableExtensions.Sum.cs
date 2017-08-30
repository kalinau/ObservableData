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
        #region int
        [NotNull]
        public static IObservable<int> SumItems(
            [NotNull] this IObservable<IBatch<GeneralChange<int>>> previous)
        {
            return Observable.Create<int>(observer =>
            {
                if (observer == null) return Disposable.Empty;

                var adapter = new Sum.GeneralChangesObserver<int>(
                    observer,
                    (x, y) => x + y,
                    (x, y) => x - y,
                    0);
                return previous.Subscribe(adapter);
            }).NotNull();
        }

        [NotNull]
        public static IObservable<int> SumItems<T>(
            [NotNull] this IObservable<IBatch<GeneralChange<T>>> previous, 
            [NotNull] Func<T, int> selector)
        {
            return previous.SelectConstantFromItems(selector).SumItems();
        }

        [NotNull]
        public static IObservable<int> SumItems(
            [NotNull] this IObservable<IBatch<IndexedChange<int>>> previous)
        {
            return previous.SelectGeneralChanges().SumItems();
        }

        [NotNull]
        public static IObservable<int> SumItems<T>(
            [NotNull] this IObservable<IBatch<IndexedChange<T>>> previous,
            [NotNull] Func<T, int> selector)
        {
            return previous.SelectConstantFromItems(selector).SumItems();
        }
        #endregion

        #region double
        [NotNull]
        public static IObservable<double> SumItems(
            [NotNull] this IObservable<IBatch<GeneralChange<double>>> previous)
        {
            return Observable.Create<double>(observer =>
            {
                if (observer == null) return Disposable.Empty;

                var adapter = new Sum.GeneralChangesObserver<double>(
                    observer,
                    (x, y) => x + y,
                    (x, y) => x - y,
                    0);
                return previous.Subscribe(adapter);
            }).NotNull();
        }

        [NotNull]
        public static IObservable<double> SumItems<T>(
            [NotNull] this IObservable<IBatch<GeneralChange<T>>> previous,
            [NotNull] Func<T, double> selector)
        {
            return previous.SelectConstantFromItems(selector).SumItems();
        }

        [NotNull]
        public static IObservable<double> SumItems(
            [NotNull] this IObservable<IBatch<IndexedChange<double>>> previous)
        {
            return previous.SelectGeneralChanges().SumItems();
        }

        [NotNull]
        public static IObservable<double> SumItems<T>(
            [NotNull] this IObservable<IBatch<IndexedChange<T>>> previous,
            [NotNull] Func<T, double> selector)
        {
            return previous.SelectConstantFromItems(selector).SumItems();
        }
        #endregion

        #region decimal
        [NotNull]
        public static IObservable<decimal> SumItems(
            [NotNull] this IObservable<IBatch<GeneralChange<decimal>>> previous)
        {
            return Observable.Create<decimal>(observer =>
            {
                if (observer == null) return Disposable.Empty;

                var adapter = new Sum.GeneralChangesObserver<decimal>(
                    observer,
                    (x, y) => x + y,
                    (x, y) => x - y,
                    0);
                return previous.Subscribe(adapter);
            }).NotNull();
        }

        [NotNull]
        public static IObservable<decimal> SumItems<T>(
            [NotNull] this IObservable<IBatch<GeneralChange<T>>> previous,
            [NotNull] Func<T, decimal> selector)
        {
            return previous.SelectConstantFromItems(selector).SumItems();
        }

        [NotNull]
        public static IObservable<decimal> SumItems(
            [NotNull] this IObservable<IBatch<IndexedChange<decimal>>> previous)
        {
            return previous.SelectGeneralChanges().SumItems();
        }

        [NotNull]
        public static IObservable<decimal> SumItems<T>(
            [NotNull] this IObservable<IBatch<IndexedChange<T>>> previous,
            [NotNull] Func<T, decimal> selector)
        {
            return previous.SelectConstantFromItems(selector).SumItems();
        }
        #endregion

        #region float
        [NotNull]
        public static IObservable<float> SumItems(
            [NotNull] this IObservable<IBatch<GeneralChange<float>>> previous)
        {
            return Observable.Create<float>(observer =>
            {
                if (observer == null) return Disposable.Empty;

                var adapter = new Sum.GeneralChangesObserver<float>(
                    observer,
                    (x, y) => x + y,
                    (x, y) => x - y,
                    0);
                return previous.Subscribe(adapter);
            }).NotNull();
        }

        [NotNull]
        public static IObservable<float> SumItems<T>(
            [NotNull] this IObservable<IBatch<GeneralChange<T>>> previous,
            [NotNull] Func<T, float> selector)
        {
            return previous.SelectConstantFromItems(selector).SumItems();
        }

        [NotNull]
        public static IObservable<float> SumItems(
            [NotNull] this IObservable<IBatch<IndexedChange<float>>> previous)
        {
            return previous.SelectGeneralChanges().SumItems();
        }

        [NotNull]
        public static IObservable<float> SumItems<T>(
            [NotNull] this IObservable<IBatch<IndexedChange<T>>> previous,
            [NotNull] Func<T, float> selector)
        {
            return previous.SelectConstantFromItems(selector).SumItems();
        }
        #endregion

        #region long
        [NotNull]
        public static IObservable<long> SumItems(
            [NotNull] this IObservable<IBatch<GeneralChange<long>>> previous)
        {
            return Observable.Create<long>(observer =>
            {
                if (observer == null) return Disposable.Empty;

                var adapter = new Sum.GeneralChangesObserver<long>(
                    observer,
                    (x, y) => x + y,
                    (x, y) => x - y,
                    0);
                return previous.Subscribe(adapter);
            }).NotNull();
        }

        [NotNull]
        public static IObservable<long> SumItems<T>(
            [NotNull] this IObservable<IBatch<GeneralChange<T>>> previous,
            [NotNull] Func<T, long> selector)
        {
            return previous.SelectConstantFromItems(selector).SumItems();
        }

        [NotNull]
        public static IObservable<long> SumItems(
            [NotNull] this IObservable<IBatch<IndexedChange<long>>> previous)
        {
            return previous.SelectGeneralChanges().SumItems();
        }

        [NotNull]
        public static IObservable<long> SumItems<T>(
            [NotNull] this IObservable<IBatch<IndexedChange<T>>> previous,
            [NotNull] Func<T, long> selector)
        {
            return previous.SelectConstantFromItems(selector).SumItems();
        }
        #endregion
    }
}