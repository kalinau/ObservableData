using System.Collections.Generic;
using JetBrains.Annotations;

namespace ObservableData.Querying
{
    public struct CollectionChangePlusState<T>
    {
        [NotNull] private readonly IChange<CollectionOperation<T>> _change;
        [NotNull] private readonly IReadOnlyCollection<T> _reachedState;

        public CollectionChangePlusState(
            [NotNull] IChange<CollectionOperation<T>> change,
            [NotNull] IReadOnlyCollection<T> reachedState)
        {
            _reachedState = reachedState;
            _change = change;
        }

        [NotNull]
        public IReadOnlyCollection<T> ReachedState => _reachedState;

        [NotNull]
        public IChange<CollectionOperation<T>> Change => _change;
    }


    [PublicAPI]
    public static class CollectionChangePlusStateExtensions
    {
        public static void ApplyTo<T>(
            this CollectionChangePlusState<T> changePlusState,
            [NotNull] ICollection<T> collection)
        {
            changePlusState.Change.ApplyTo(collection);
        }
    }
}