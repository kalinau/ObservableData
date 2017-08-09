using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using ObservableData.Querying.Compatibility;
using ObservableData.Structures;

namespace ObservableData.Querying
{
    [PublicAPI]
    public static class ListExtensions
    {
        [NotNull]
        public static IQuery<T> AsQuery<T>(
            [NotNull] this IObservableReadOnlyList<T> list) => 
            new ListToQueryAdapter<T>(list);

        [NotNull]
        public static IEnumerable<T> AsBindableList<T>(
            [NotNull] this IObservableReadOnlyList<T> list) =>
            list.AsQuery().AsBindableList();

        //[NotNull]
        //public static IQuery<TOut> SelectImmutable<TIn, TOut>(
        //    [NotNull] this IObservableReadOnlyList<TIn> list,
        //    [NotNull] Func<TIn, TOut> func) =>
        //    list.AsQuery().SelectImmutable(func);

        [NotNull]
        public static IQuery<T> AsWeakQuery<T>(
            [NotNull] this IObservableReadOnlyList<T> list) =>
            list.AsQuery().AsWeak();

        [NotNull]
        public static IQuery<TOut> SelectConstant<TIn, TOut>(
            [NotNull] this IObservableReadOnlyList<TIn> list,
            [NotNull] Func<TIn, TOut> func) =>
            list.AsQuery().SelectConstant(func);

        //[NotNull]
        //public static IQuery<TIn> WhereImmutable<TIn>(
        //    [NotNull] this IObservableReadOnlyList<TIn> list,
        //    [NotNull] Func<TIn, bool> func) =>
        //    list.AsQuery().WhereImmutable(func);
    }
}
