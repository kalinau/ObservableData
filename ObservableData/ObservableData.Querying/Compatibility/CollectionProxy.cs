using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace ObservableData.Querying.Compatibility
{
    [PublicAPI]
    public abstract class CollectionProxy<T> : IReadOnlyCollection<T>
    {
        [NotNull] private static readonly IReadOnlyCollection<T> Default = new List<T>(0);
        [NotNull] private IReadOnlyCollection<T> _subject = Default;

        [NotNull]
        public IReadOnlyCollection<T> Subject
        {
            get => _subject;
            set => _subject = value;
        }

        public IEnumerator<T> GetEnumerator() => _subject.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        public int Count => _subject.Count;
    }
}