using System.Collections.Generic;
using JetBrains.Annotations;

namespace ObservableData.Querying
{
    public interface IListChangeEnumerator<in T>
    {
        //void OnStateChanged([NotNull] IReadOnlyList<T> state);

        void OnClear();

        void OnAdd(T item, int index);

        void OnRemove(T item, int index);

        void OnMove(T item, int index, int originalIndex);

        void OnReplace(T item, T changedItem, int index);
    }
}