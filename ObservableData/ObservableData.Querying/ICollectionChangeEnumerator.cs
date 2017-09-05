using System.Collections.Generic;
using JetBrains.Annotations;

namespace ObservableData.Querying
{
    public interface ICollectionChangeEnumerator<in T>
    {
        void OnStateChanged([NotNull] IReadOnlyCollection<T> state);

        void OnClear();

        void OnAdd(T item);

        void OnRemove(T item);
    }
}