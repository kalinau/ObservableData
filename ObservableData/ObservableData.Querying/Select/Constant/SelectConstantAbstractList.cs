using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using ObservableData.Querying.Utils.Adapters.ToList;

namespace ObservableData.Querying.Select.Constant
{
    public sealed class SelectConstantList<TIn, TOut> : ListAdapter<TIn, TOut>
    {
        [NotNull] private readonly Func<TIn, TOut> _func;

        public SelectConstantList(
            [NotNull]  IReadOnlyList<TIn> source,
            [NotNull] Func<TIn, TOut> func)
            :base(source)
        {
            _func = func;
        }

        protected override IEnumerable<TOut> Enumerate(IEnumerable<TIn> source) =>
            source.Select(_func);

        protected override TOut Select(TIn item) => _func(item);
    }
}