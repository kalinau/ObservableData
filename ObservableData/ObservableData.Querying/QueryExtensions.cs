using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using ObservableData.Querying.Compatibility;
using ObservableData.Querying.Select.Constant;
using ObservableData.Querying.Weak;

namespace ObservableData.Querying
{
    [PublicAPI]
    public static class QueryExtensions
    {
        //[NotNull]
        //public static IQuery<TOut> SelectImmutable<TIn, TOut>(
        //    [NotNull] this IQuery<TIn> list, [NotNull] Func<TIn, TOut> func) => 
        //    new SelectImmutableQuery<TIn, TOut>(list, func);

        [NotNull]
        public static IQuery<T> SelectMutable<T>(
            [NotNull] this IQuery<T> list)
        {
            throw new NotImplementedException();
        }

        [NotNull]
        public static IQuery<TOut> SelectConstant<TIn, TOut>(
            [NotNull] this IQuery<TIn> data,
            [NotNull] Func<TIn, TOut> func) =>
            new SelectConstantQuery<TIn, TOut>(data, func);

        //[NotNull]
        //public static IObservable<IChange<CollectionOperation<T>>> ForFilteredByImmutable<T>(
        //    [NotNull] this IObservable<IChange<CollectionOperation<T>>> whenUpdated,
        //    [NotNull] Func<T, bool> func) =>
        //    new WhereByImmutableQuery<T>(whenUpdated, func);

        [NotNull]
        public static IQuery<T> AsWeak<T>(
            [NotNull] this IQuery<T> query) =>
            new WeakSubscribableQuery<T>(query);

        [NotNull]
        public static IEnumerable<T> AsBindableList<T>(
            [NotNull] this IQuery<T> data) =>
            new QueryAsBindableListAdapter<T>(data);

        [NotNull]
        public static QueryAsObservableListAdapter<T> AsObservableList<T>(
            [NotNull] this IQuery<T> query) =>
            new QueryAsObservableListAdapter<T>(query);
    }
}