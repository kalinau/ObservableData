using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using JetBrains.Annotations;
using ObservableData.Querying.Utils;
using ObservableData.Querying.Where;

namespace ObservableData.Querying
{
    public static partial class ObservableExtensions
    {
        [NotNull]
        public static IObservable<IBatch<GeneralChange<T>>> WhereItems<T>(
            [NotNull] this IObservable<IBatch<GeneralChange<T>>> previous,
            [NotNull] Func<T, bool> criterion)
        {
            return Observable.Create<IBatch<GeneralChange<T>>>(o =>
            {
                if (o == null) return Disposable.Empty;

                var adapter = new WhereByImmutable.GeneralChangesObserver<T>(o, criterion);
                return previous.Subscribe(adapter);
            }).NotNull();
        }

        [NotNull]
        public static IObservable<GeneralChangesPlusState<T>> WhereItems<T>(
            [NotNull] this IObservable<GeneralChangesPlusState<T>> previous,
            [NotNull] Func<T, bool> criterion)
        {
            return Observable.Create<GeneralChangesPlusState<T>>(o =>
            {
                if (o == null) return Disposable.Empty;

                var adapter = new WhereByImmutable.GeneralChangesPlusStateObserver<T>(o, criterion);
                return previous.Subscribe(adapter);
            }).NotNull();
        }
    }
}