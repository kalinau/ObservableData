using System.Collections.Generic;
using JetBrains.Annotations;

namespace ObservableData.Querying
{
    public struct GeneralChangesPlusState<T>
    {
        [NotNull] private readonly IBatch<GeneralChange<T>> _change;
        [NotNull] private readonly IReadOnlyCollection<T> _reachedState;

        public GeneralChangesPlusState(
            [NotNull] IBatch<GeneralChange<T>> changes,
            [NotNull] IReadOnlyCollection<T> reachedState)
        {
            _reachedState = reachedState;
            _change = changes;
        }

        [NotNull]
        public IReadOnlyCollection<T> ReachedState => _reachedState;

        [NotNull]
        public IBatch<GeneralChange<T>> Changes => _change;
    }

    [PublicAPI]
    public static class GeneralChangesPlusStateExtensions
    {
        public static void ApplyTo<T>(
            this GeneralChangesPlusState<T> changesPlusState,
            [NotNull] ICollection<T> collection)
        {
            changesPlusState.Changes.ApplyTo(collection);
        }
    }
}