using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using ObservableData.Querying.Utils.Adapters;

namespace ObservableData.Querying.Select.Constant
{
    internal sealed class SelectConstantList<TIn, TOut> : ListAdapter<TIn, TOut>
    {
        [NotNull] private readonly Func<TIn,TOut> _select;

        public SelectConstantList([NotNull] IReadOnlyList<TIn> source, [NotNull] Func<TIn, TOut> select) : base(source)
        {
            _select = select;
        }

        protected override IEnumerable<TOut> Enumerate(IEnumerable<TIn> source) => source.Select(_select);

        protected override TOut Select(TIn item) => _select(item);
    }
}