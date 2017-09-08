using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace ObservableData.Querying
{
    public sealed class ListToCollectionChangeEnumerator<T> : IListChangeEnumerator<T>
    {
        [NotNull] private ICollectionChangeEnumerator<T> _adaptee;

        public ListToCollectionChangeEnumerator([NotNull] ICollectionChangeEnumerator<T> adaptee)
        {
            _adaptee = adaptee;
        }

        public void ChangeEnumerator([NotNull] ICollectionChangeEnumerator<T> enumerator)
        {
            _adaptee = enumerator;
        }

        public void OnStateChanged(IReadOnlyList<T> state) => 
            _adaptee.OnStateChanged(state);

        public void OnClear() => _adaptee.OnClear();

        public void OnAdd(T item, int index) => _adaptee.OnAdd(item, index);

        public void OnRemove(T item, int index) => _adaptee.OnRemove(item, index);

        public void OnMove(T item, int index, int originalIndex) => _adaptee.OnMove(item,  index, originalIndex);

        public void OnReplace(T item, T changedItem, int index) => _adaptee.OnReplace(item, changedItem, index);
    }

    public static class CollectionChangeEnumeratorExtensions
    {
        [ContractAnnotation("=>buffer:notnull; => notnull")]
        public static IListChangeEnumerator<T> FromBuffer<T>(
            [NotNull] this ICollectionChangeEnumerator<T> enumerator,
            ref ListToCollectionChangeEnumerator<T> buffer)
        {
            if (buffer == null)
            {
                buffer = new ListToCollectionChangeEnumerator<T>(enumerator);
            }
            else
            {
                buffer.ChangeEnumerator(enumerator);
            }
            return buffer;
        }

        [SuppressMessage("ReSharper", "UnusedParameter.Global")]
        public static void OnAdd<T>(
            [NotNull] this ICollectionChangeEnumerator<T> enumerator,
            T item,
            int index) => enumerator.OnAdd(item);

        [SuppressMessage("ReSharper", "UnusedParameter.Global")]
        public static void OnRemove<T>(
            [NotNull] this ICollectionChangeEnumerator<T> enumerator,
            T item,
            int index) => enumerator.OnRemove(item);

        [SuppressMessage("ReSharper", "UnusedParameter.Global")]
        public static void OnMove<T>(
            [NotNull] this ICollectionChangeEnumerator<T> enumerator,
            T item,
            int index,
            int originalIndex) { }

        [SuppressMessage("ReSharper", "UnusedParameter.Global")]
        public static void OnReplace<T>(
            [NotNull] this ICollectionChangeEnumerator<T> enumerator, 
            T item, 
            T changedItem,
            int index)
        {
            enumerator.OnRemove(changedItem);
            enumerator.OnAdd(item);
        }
    }
}