using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using ObservableData.Querying.Compatibility;

namespace ObservableData.Querying
{
    public interface ICollectionChange<out T>
    {
        void Enumerate([NotNull] ICollectionChangeEnumerator<T> enumerator);
    }

    public interface ICollectionChangeEnumerator<in T>
    {
        void OnStateChanged([NotNull] IReadOnlyCollection<T> state);

        void OnClear();

        void OnAdd(T item);

        void OnRemove(T item);
    }

    public interface IListChange<out T> : ICollectionChange<T>
    {
        void Enumerate([NotNull] IListChangeEnumerator<T> enumerator);
    }

    public interface IListChangeEnumerator<in T>
    {
        void OnStateChanged([NotNull] IReadOnlyList<T> state);

        void OnClear();

        void OnAdd(T item, int index);

        void OnRemove(T item, int index);

        void OnMove(T item, int index, int originalIndex);

        void OnReplace(T item, T changedItem, int index);
    }



    public sealed class ListToCollectionChangeEnumerator<T> : IListChangeEnumerator<T>
    {
        [NotNull] private ICollectionChangeEnumerator<T> _adaptee;

        public ListToCollectionChangeEnumerator([NotNull] ICollectionChangeEnumerator<T> adaptee)
        {
            _adaptee = adaptee;
        }

        public void ChangeAdaptee([NotNull] ICollectionChangeEnumerator<T> adaptee)
        {
            _adaptee = adaptee;
        }

        public void OnStateChanged(IReadOnlyList<T> state) => _adaptee.OnStateChanged(state);

        public void OnClear() => _adaptee.OnClear();

        public void OnAdd(T item, int index) => _adaptee.OnAdd(item);

        public void OnRemove(T item, int index) => _adaptee.OnRemove(item);

        public void OnMove(T item, int index, int originalIndex) { }

        public void OnReplace(T item, T changedItem, int index)
        {
            _adaptee.OnRemove(changedItem);
            _adaptee.OnAdd(item);
        }
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
                buffer.ChangeAdaptee(enumerator);
            }
            return buffer;
        }
    }

    public interface IBatch<out T>
    {
        [NotNull, ItemNotNull]
        IEnumerable<T> GetPeaces();

        void MakeImmutable();
    }

    public static class BatchExtensions
    {
        public static void ApplyTo<T>(
            [NotNull] this IBatch<IndexedChange<T>> change,
            [NotNull] NotifyCollectionEvents<T> events)
        {
            if (events.HasObservers)
            {
                foreach (var update in change.GetPeaces())
                {
                    events.OnChange(update);
                    if (update.Type == IndexedChangeType.Clear)
                    {
                        break;
                    }
                }
            }
        }
    }
}