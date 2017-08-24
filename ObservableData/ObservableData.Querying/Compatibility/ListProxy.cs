using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace ObservableData.Querying.Compatibility
{
    [PublicAPI]
    public class ListProxy<T> : IReadOnlyList<T>
    {
        [NotNull] private static readonly IReadOnlyList<T> Default = new List<T>(0);
        [NotNull] private IReadOnlyList<T> _subject = Default;

        [NotNull]
        protected IReadOnlyList<T> Subject
        {
            get => _subject;
            set => _subject = value;
        }

        public IEnumerator<T> GetEnumerator() => _subject.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        public int Count => _subject.Count;

        public T this[int index] => _subject[index];
    }
}