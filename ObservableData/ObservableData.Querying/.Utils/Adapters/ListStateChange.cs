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
            foreach (var item in _state)
            {
                enumerator.OnAdd(item);
            }
        }

        void IListChange<T>.Enumerate(IListChangeEnumerator<T> enumerator)
        {
            var i = 0;
            foreach (var item in _state)
            {
                enumerator.OnAdd(item, i++);
            }
        }
    }
}