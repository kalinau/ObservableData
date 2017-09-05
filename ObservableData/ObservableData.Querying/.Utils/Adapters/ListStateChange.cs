using System.Collections.Generic;
using JetBrains.Annotations;

namespace ObservableData.Querying.Utils.Adapters
{
    public class ListStateChange<T> : ICollectionChange<T>, IListChange<T>
    {
        [NotNull] private readonly IReadOnlyList<T> _state;

        public ListStateChange([NotNull] IReadOnlyList<T> state)
        {
            _state = state;
        }

        void ICollectionChange<T>.Enumerate(ICollectionChangeEnumerator<T> enumerator)
        {
            enumerator.OnStateChanged(_state);
        }

        void IListChange<T>.Enumerate(IListChangeEnumerator<T> enumerator)
        {
            enumerator.OnStateChanged(_state);
        }
    }
}