using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace ObservableData.Querying.Utils
{
    [PublicAPI]
    public class ListProxy<T> : IReadOnlyList<T>
    {
        [NotNull] private static readonly IReadOnlyList<T> Default = new List<T>(0);
        [NotNull] private IReadOnlyList<T> _underlyingList = Default;

        [NotNull]
        public IReadOnlyList<T> UnderlyingList
        {
            get => _underlyingList;
            set => _underlyingList = value;
        }

        public IEnumerator<T> GetEnumerator() => _underlyingList.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        public int Count => _underlyingList.Count;

        public T this[int index] => _underlyingList[index];
    }
}