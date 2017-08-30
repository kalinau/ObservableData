using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using JetBrains.Annotations;
using ObservableData.Querying.Boolean;
using ObservableData.Querying.Utils;

namespace ObservableData.Querying
{
    public static partial class ObservableExtensions
    {
        [NotNull]
        public static IObservable<bool> AllItems<T>(
            [NotNull] this IObservable<IBatch<GeneralChange<T>>> previous,
            [NotNull] Func<T, bool> criterion)
        {
            return Observable.Create<bool>(observer =>
            {
                if (observer == null) return Disposable.Empty;

                var adapter = new All.GeneralChangesObserver<T>(observer, criterion);
                return previous.Subscribe(adapter);
            }).NotNull();
        }

        [NotNull]
        public static IObservable<bool> AllItems<T>(
            [NotNull] this IObservable<IBatch<IndexedChange<T>>> previous,
            [NotNull] Func<T, bool> criterion)
        {
            return previous.SelectGeneralChanges().AllItems(criterion);
        }

        [NotNull]
        public static IObservable<bool> AnyItem<T>(
            [NotNull] this IObservable<IBatch<GeneralChange<T>>> previous,
            [NotNull] Func<T, bool> criterion)
        {
            return Observable.Create<bool>(observer =>
            {
                if (observer == null) return Disposable.Empty;

                var adapter = new Any.GeneralChangesObserver<T>(observer, criterion);
                return previous.Subscribe(adapter);
            }).NotNull();
        }

        [NotNull]
        public static IObservable<bool> AnyItem<T>(
            [NotNull] this IObservable<IBatch<IndexedChange<T>>> previous,
            [NotNull] Func<T, bool> criterion)
        {
            return previous.SelectGeneralChanges().AnyItem(criterion);
        }

        [NotNull]
        public static IObservable<bool> ContainsItem<T>(
            [NotNull] this IObservable<IBatch<GeneralChange<T>>> previous,
            T value,
            [NotNull] IEqualityComparer<T> comparer)
        {
            return previous.AnyItem(x => comparer.Equals(x, value));
        }

        [NotNull]
        public static IObservable<bool> ContainsItem<T>(
            [NotNull] this IObservable<IBatch<GeneralChange<T>>> previous,
            T value)
        {
            return previous.ContainsItem(value, EqualityComparer<T>.Default.NotNull());
        }

        [NotNull]
        public static IObservable<bool> ContainsItem<T>(
            [NotNull] this IObservable<IBatch<IndexedChange<T>>> previous,
            T value,
            [NotNull] IEqualityComparer<T> comparer)
        {
            return previous.AnyItem(x => comparer.Equals(x, value));
        }

        [NotNull]
        public static IObservable<bool> ContainsItem<T>(
            [NotNull] this IObservable<IBatch<IndexedChange<T>>> previous,
            T value)
        {
            return previous.ContainsItem(value, EqualityComparer<T>.Default.NotNull());
        }

    }
}