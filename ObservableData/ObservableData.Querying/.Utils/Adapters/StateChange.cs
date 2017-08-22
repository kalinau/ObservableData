using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace ObservableData.Querying.Utils.Adapters
{
    public class StateChange<T> : 
        IChange<CollectionOperation<T>>,
        IChange<ListOperation<T>>
    {
        [NotNull] private readonly IEnumerable<T> _state;

        public StateChange([NotNull] IEnumerable<T> state)
        {
            _state = state;
        }

        IEnumerable<ListOperation<T>> IChange<ListOperation<T>>.Iterations()
        {
            var i = 0;
            foreach (var item in _state)
            {
                yield return ListOperation<T>.OnAdd(item, i++);
            }
        }

        IEnumerable<CollectionOperation<T>> IChange<CollectionOperation<T>>.Iterations()
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
