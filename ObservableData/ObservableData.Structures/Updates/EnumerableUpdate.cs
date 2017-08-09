using System.Collections.Generic;
using System.Linq;
using ObservableData.Querying.Utils;

namespace ObservableData.Structures.Updates
{
    public abstract class EnumerableUpdate<T> : IQuickLinkableUpdate<T>
    {
        public abstract void Lock();

        public abstract IEnumerable<T> Operations();

        bool IQuickLinkableUpdate<T>.IsSingle => false;

        T IQuickLinkableUpdate<T>.First => this.Operations().FirstOrDefault();

        IQuickLinkableUpdate<T> IQuickLinkableUpdate<T>.Next { get; set; }
    }
}
