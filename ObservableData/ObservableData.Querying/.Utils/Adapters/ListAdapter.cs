using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using ObservableData.Structures;

namespace ObservableData.Querying.Utils.Adapters
{
    public abstract class ListAdapter<TIn, TOut> : IReadOnlyList<TOut>
    {
        [NotNull] private readonly IReadOnlyList<TIn> _source;

        protected ListAdapter([NotNull] IReadOnlyList<TIn> source)
        {
            _source = source;
        }

        public int Count => _source.Count;

        public IEnumerator<TOut> GetEnumerator() => this.Enumerate(_source).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        public TOut this[int index] => this.Select(_source[index]);

        [NotNull]
        protected abstract IEnumerable<TOut> Enumerate([NotNull] IEnumerable<TIn> source);

        protected abstract TOut Select(TIn item);
    }
}
