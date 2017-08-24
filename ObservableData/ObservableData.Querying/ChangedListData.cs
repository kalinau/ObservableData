using System.Collections.Generic;
using JetBrains.Annotations;

namespace ObservableData.Querying
{
    public struct ListChangePlusState<T>
    {
        [NotNull] private readonly IReadOnlyList<T> _reachedState;
        [NotNull] private readonly IChange<ListOperation<T>> _change;

        public ListChangePlusState(
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


    [PublicAPI]
    public static class ListChangePlusStateExtensions
    {
        public static void ApplyTo<T>(
            this ListChangePlusState<T> changePlusState,
            [NotNull] IList<T> list)
        {
            changePlusState.Change.ApplyTo(list);
        }
    }
}