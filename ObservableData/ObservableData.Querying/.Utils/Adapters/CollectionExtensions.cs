using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace ObservableData.Querying.Utils.Adapters
{
    internal static class CollectionExtensions
    {
        private sealed class CollectionAdapter<TIn, TOut> : IReadOnlyCollection<TOut>
        {
            [NotNull] private readonly IReadOnlyCollection<TIn> _source;
            [NotNull] private readonly Func<IEnumerable<TIn>, IEnumerable<TOut>> _enumerate;

            public CollectionAdapter(
                [NotNull] IReadOnlyCollection<TIn> source,
                [NotNull] Func<IEnumerable<TIn>, IEnumerable<TOut>> enumerate)
            {
                _source = source;
                _enumerate = enumerate;
            }

            public int Count => _source.Count;

            public IEnumerator<TOut> GetEnumerator() => _enumerate(_source).NotNull().GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
        }

        public static IReadOnlyCollection<TIn> WhereForCollection<TIn>(
            [NotNull] this IReadOnlyCollection<TIn> adaptee,
            [NotNull] Func<TIn, bool> criterion)
        {
            return new CollectionAdapter<TIn, TIn>(adaptee, e => e.Where(criterion));
        }

        public static IReadOnlyCollection<TOut> SelectForCollection<TIn, TOut>(
            [NotNull] this IReadOnlyCollection<TIn> update,
            [NotNull] Func<TIn, TOut> selector)
        {
            return new CollectionAdapter<TIn, TOut>(update, e => e.Select(selector));
        }
    }
}