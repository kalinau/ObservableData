using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using ObservableData.Querying;
using ObservableData.Structures.Utils;

namespace ObservableData.Structures
{
    [PublicAPI]
    public interface ICollectionOperation<out T>
    {
        TResult Match<TResult>(
            [NotNull] Func<ICollectionInsertOperation<T>, TResult> onInsert,
            [NotNull] Func<ICollectionRemoveOperation<T>, TResult> onRemove,
            [NotNull] Func<ICollectionReplaceOperation<T>, TResult> onReplace,
            [NotNull] Func<ICollectionClearOperation<T>, TResult> onClear);

        void Match(
            Action<ICollectionInsertOperation<T>> onInsert,
            Action<ICollectionRemoveOperation<T>> onRemove,
            Action<ICollectionReplaceOperation<T>> onReplace,
            Action<ICollectionClearOperation<T>> onClear);
    }

    public interface ICollectionInsertOperation<out T> : ICollectionOperation<T>
    {
        [NotNull]
        IReadOnlyCollection<T> Items { get; }
    }

    public interface ICollectionRemoveOperation<out T> : ICollectionOperation<T>
    {
        T Item { get; }
    }

    public interface ICollectionReplaceOperation<out T> : ICollectionOperation<T>
    {
        T Item { get; }

        T ReplacedItem { get; }
    }

    public interface ICollectionClearOperation<out T> : ICollectionOperation<T>
    {
    }

    [PublicAPI]
    public static class CollectionOperationExtensions
    {
        [NotNull]
        public static IEnumerable<GeneralChange<T>> ToGeneralChanges<T>(
            [NotNull, ItemNotNull] this IEnumerable<ICollectionOperation<T>> changes)
        {
            foreach (var i in changes)
            {
                foreach (var o in i.ToGeneralChanges())
                {
                    yield return o;
                }
            }
        }

        [NotNull]
        public static IEnumerable<GeneralChange<T>> ToGeneralChanges<T>(
            [NotNull] this ICollectionOperation<T> operation)
        {
            return operation.Match(
                ToGeneralChanges,
                ToGeneralChanges,
                ToGeneralChanges,
                ToGeneralChanges
            ).NotNull();
        }

        [NotNull]
        public static IEnumerable<GeneralChange<T>> ToGeneralChanges<T>(
            [NotNull] this ICollectionInsertOperation<T> insert)
        {
            foreach (var item in insert.Items)
            {
                yield return GeneralChange<T>.OnAdd(item);
            }
}

        [NotNull]
        public static IEnumerable<GeneralChange<T>> ToGeneralChanges<T>(
            [NotNull] this ICollectionRemoveOperation<T> remove)
        {
            yield return GeneralChange<T>.OnRemove(remove.Item);
        }

        [NotNull]
        public static IEnumerable<GeneralChange<T>> ToGeneralChanges<T>(
            [NotNull] this ICollectionReplaceOperation<T> replace)
        {
            yield return GeneralChange<T>.OnRemove(replace.ReplacedItem);
            yield return GeneralChange<T>.OnAdd(replace.Item);
        }

        [NotNull]
        public static IEnumerable<GeneralChange<T>> ToGeneralChanges<T>(
            [NotNull] this ICollectionClearOperation<T> clear)
        {
            yield return GeneralChange<T>.OnClear();
        }
    }
}