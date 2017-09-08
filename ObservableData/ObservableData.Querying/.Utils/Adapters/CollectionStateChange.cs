using System.Collections.Generic;
using JetBrains.Annotations;

namespace ObservableData.Querying.Utils.Adapters
{
    public class CollectionStateChange<T> : ICollectionChange<T>
    {
        [NotNull] private readonly IReadOnlyCollection<T> _state;

        public CollectionStateChange([NotNull] IReadOnlyCollection<T> state)
        {
            _state = state;
        }

        public void Enumerate(ICollectionChangeEnumerator<T> enumerator)
        {
            foreach (var i in _state)
            {
                enumerator.OnAdd(i);
            }
        }
    }
}