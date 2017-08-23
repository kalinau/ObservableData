using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace ObservableData.Querying.Utils.Adapters
{
    [UsedImplicitly]
    public class StateChange<T> : 
        IChange<CollectionOperation<T>>,
        IChange<ListOperation<T>>
    {
        [NotNull] private readonly IEnumerable<T> _state;

        public StateChange([NotNull] IEnumerable<T> state)
        {
            _state = state;
        }

        IEnumerable<ListOperation<T>> IChange<ListOperation<T>>.GetIterations()
        {
            var i = 0;
            foreach (var item in _state)
            {
                yield return ListOperation<T>.OnAdd(item, i++);
            }
        }

        IEnumerable<CollectionOperation<T>> IChange<CollectionOperation<T>>.GetIterations()
        {
            foreach (var item in _state)
            {
                yield return CollectionOperation<T>.OnAdd(item);
            }
        }

        public void MakeImmutable()
        {
            throw new NotImplementedException();
        }
    }
}
