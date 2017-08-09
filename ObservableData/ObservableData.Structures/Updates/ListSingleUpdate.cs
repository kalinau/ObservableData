using System.Collections.Generic;
using ObservableData.Querying.Core;

namespace ObservableData.Structures.Updates
{
    public abstract class ListSingleUpdate<T> : IQuickLinkableUpdate<ListOperation<T>>
    {
        public void Lock() { }

        public IEnumerable<ListOperation<T>> Operations()
        {
            yield return this.First;
        }

        public bool IsSingle => true;

        public abstract ListOperation<T> First { get; }

        IQuickLinkableUpdate<ListOperation<T>> IQuickLinkableUpdate<ListOperation<T>>.Next { get; set; }
    }
}