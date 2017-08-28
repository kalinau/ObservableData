using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace ObservableData.Querying.Utils.Adapters
{
    [UsedImplicitly]
    public class StateChange<T> : 
        IBatch<GeneralChange<T>>,
        IBatch<IndexedChange<T>>
    {
        [NotNull] private readonly IEnumerable<T> _state;

        public StateChange([NotNull] IEnumerable<T> state)
        {
            _state = state;
        }

        IEnumerable<IndexedChange<T>> IBatch<IndexedChange<T>>.GetIterations()
        {
            var i = 0;
            foreach (var item in _state)
            {
                yield return IndexedChange<T>.OnAdd(item, i++);
            }
        }

        IEnumerable<GeneralChange<T>> IBatch<GeneralChange<T>>.GetIterations()
        {
            foreach (var item in _state)
            {
                yield return GeneralChange<T>.OnAdd(item);
            }
        }

        public void MakeImmutable()
        {
            throw new NotImplementedException();
        }
    }
}
