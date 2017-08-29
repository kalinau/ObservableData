using System.Collections.Generic;
using JetBrains.Annotations;

namespace ObservableData.Querying
{
    public struct IndexedChangesPlusState<T>
    {
        [NotNull] private readonly IReadOnlyList<T> _reachedState;
        [NotNull] private readonly IBatch<IndexedChange<T>> _change;

        public IndexedChangesPlusState(
            [NotNull] IBatch<IndexedChange<T>> changes,
            [NotNull] IReadOnlyList<T> reachedState)
        {
            _reachedState = reachedState;
            _change = changes;
        }

        [NotNull]
        public IReadOnlyList<T> ReachedState => _reachedState;

        [NotNull]
        public IBatch<IndexedChange<T>> Change => _change;
    }

    [PublicAPI]
    public static class IndexedChangesPlusStateExtensions
    {
        public static void ApplyTo<T>(
            this IndexedChangesPlusState<T> changesPlusState,
            [NotNull] IList<T> list)
        {
            changesPlusState.Change.ApplyTo(list);
        }
    }
}