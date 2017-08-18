using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using ObservableData.Querying.Compatibility;

namespace ObservableData.Querying
{
    [PublicAPI]
    public static class QueryExtensions
    {
        //[NotNull]
        //public static IQuery<TOut> SelectImmutable<TIn, TOut>(
        //    [NotNull] this IQuery<TIn> list, [NotNull] Func<TIn, TOut> func) => 
        //    new SelectImmutableQuery<TIn, TOut>(list, func);

        //[NotNull]
        //public static IQuery<T> SelectMutable<T>(
        //    [NotNull] this IQuery<T> list)
        //{
        //    throw new NotImplementedException();
        //}

        //[NotNull]
        //public static IQuery<TOut> SelectConstant<TIn, TOut>(
        //    [NotNull] this IQuery<TIn> data,
        //    [NotNull] Func<TIn, TOut> func) =>
        //    new SelectConstantQuery<TIn, TOut>(data, func);

        //[NotNull]
        //public static IObservable<IChange<CollectionOperation<T>>> ForFilteredByImmutable<T>(
        //    [NotNull] this IObservable<IChange<CollectionOperation<T>>> whenUpdated,
        //    [NotNull] Func<T, bool> func) =>
        //    new WhereByImmutableQuery<T>(whenUpdated, func);

        //[NotNull]
        //public static IEnumerable<T> AsBindableList<T>(
        //    [NotNull] this IQuery<T> data) =>
        //    new QueryAsBindableListAdapter<T>(data);

        //[NotNull]
        //public static QueryAsObservableListAdapter<T> AsObservableList<T>(
        //    [NotNull] this IQuery<T> query) =>
        //    new QueryAsObservableListAdapter<T>(query);
    }
}