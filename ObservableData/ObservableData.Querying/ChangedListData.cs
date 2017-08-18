using System.Collections.Generic;
using JetBrains.Annotations;
using ObservableData.Structures;

namespace ObservableData.Querying
{
    public struct ChangedListData<T>
    {
        [NotNull] private readonly IReadOnlyList<T> _reachedState;
        [NotNull] private readonly IChange<ListOperation<T>> _change;

        public ChangedListData(
            [NotNull] IChange<ListOperation<T>> change,
            [NotNull] IReadOnlyList<T> reachedState)
        {
            _reachedState = reachedState;
            _change = change;
        }

        [NotNull]
        public IReadOnlyList<T> ReachedState => _reachedState;

        [NotNull]
        public IChange<ListOperation<T>> Change => _change;
    }
}