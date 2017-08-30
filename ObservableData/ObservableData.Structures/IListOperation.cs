using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using ObservableData.Querying;
using ObservableData.Querying.Compatibility;
using ObservableData.Structures.Lists;
using ObservableData.Structures.Utils;

namespace ObservableData.Structures
{
    [PublicAPI]
    public interface IListOperation<out T>
    {
        TResult Match<TResult>(
            [NotNull] Func<IListInsertOperation<T>, TResult> onInsert,
            [NotNull] Func<IListRemoveOperation<T>, TResult> onRemove,
            [NotNull] Func<IListReplaceOperation<T>, TResult> onReplace,
            [NotNull] Func<IListMoveOperation<T>, TResult> onMove,
            [NotNull] Func<IListClearOperation<T>, TResult> onClear);

        void Match(
            Action<IListInsertOperation<T>> onInsert,
            Action<IListRemoveOperation<T>> onRemove,
            Action<IListReplaceOperation<T>> onReplace,
            Action<IListMoveOperation<T>> onMove,
            Action<IListClearOperation<T>> onClear);
    }

    [PublicAPI]
    public interface IListInsertOperation<out T> : IListOperation<T>
    {
        int Index { get; }

        [NotNull]
        IReadOnlyCollection<T> Items { get; }
    }

    [PublicAPI]
    public interface IListRemoveOperation<out T> : IListOperation<T>
    {
        int Index { get; }

        T Item { get; }
    }

    [PublicAPI]
    public interface IListReplaceOperation<out T> : IListOperation<T>
    {
        int Index { get; }

        T Item { get; }

        T ReplacedItem { get; }
    }

    [PublicAPI]
    public interface IListMoveOperation<out T> : IListOperation<T>
    {
        T Item { get; }

        int From { get; }

        int To { get; }
    }

    [PublicAPI]
    public interface IListClearOperation<out T> : IListOperation<T>
    {
    }

    [PublicAPI]
    public static class ListOperationExtensions
    {
        [NotNull]
        public static IEnumerable<IndexedChange<T>> ToIndexedChanges<T>(
            [NotNull, ItemNotNull] this IEnumerable<IListOperation<T>> changes)
        {
            foreach (var i in changes)
            {
                var operations = i.ToIndexedChanges();
                foreach (var o in operations)
                {
                    {
                        yield return o;
                    }
                }
            }
        }

        [NotNull]
        public static IEnumerable<IndexedChange<T>> ToIndexedChanges<T>(
            [NotNull] this IListOperation<T> operation)
        {
            return operation.Match(
                ToIndexedChanges,
                ToIndexedChanges,
                ToIndexedChanges,
                ToIndexedChanges,
                ToIndexedChanges
            ).NotNull();
        }

        [NotNull]
        public static IEnumerable<IndexedChange<T>> ToIndexedChanges<T>(
            [NotNull] this IListInsertOperation<T> insert)
        {
            var index = insert.Index;
            foreach (var item in insert.Items)
            {
                yield return IndexedChange<T>.OnAdd(item, index++);
            }
        }

        [NotNull]
        public static IEnumerable<IndexedChange<T>> ToIndexedChanges<T>(
            [NotNull] this IListRemoveOperation<T> remove)
        {
            yield return IndexedChange<T>.OnRemove(remove.Item, remove.Index);
        }

        [NotNull]
        public static IEnumerable<IndexedChange<T>> ToIndexedChanges<T>(
            [NotNull] this IListReplaceOperation<T> replace)
        {
            yield return IndexedChange<T>.OnReplace(replace.Item, replace.ReplacedItem, replace.Index);
        }

        [NotNull]
        public static IEnumerable<IndexedChange<T>> ToIndexedChanges<T>(
            [NotNull] this IListMoveOperation<T> move)
        {
            yield return IndexedChange<T>.OnMove(move.Item, move.To, move.From);
        }

        [NotNull]
        public static IEnumerable<IndexedChange<T>> ToIndexedChanges<T>(
            [NotNull] this IListClearOperation<T> clear)
        {
            yield return IndexedChange<T>.OnClear();
        }

        [NotNull]
        public static IEnumerable<GeneralChange<T>> ToGeneralChanges<T>(
            [NotNull, ItemNotNull] this IEnumerable<IListOperation<T>> changes)
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
            [NotNull] this IListOperation<T> operation)
        {
            return operation.Match(
                ToGeneralChanges,
                ToGeneralChanges,
                ToGeneralChanges,
                ToGeneralChanges,
                ToGeneralChanges
            ).NotNull();
        }

        [NotNull]
        public static IEnumerable<GeneralChange<T>> ToGeneralChanges<T>(
            [NotNull] this IListInsertOperation<T> insert)
        {
            foreach (var item in insert.Items)
            {
                yield return GeneralChange<T>.OnAdd(item);
            }
        }

        [NotNull]
        public static IEnumerable<GeneralChange<T>> ToGeneralChanges<T>(
            [NotNull] this IListRemoveOperation<T> remove)
        {
            yield return GeneralChange<T>.OnRemove(remove.Item);
        }

        [NotNull]
        public static IEnumerable<GeneralChange<T>> ToGeneralChanges<T>(
            [NotNull] this IListReplaceOperation<T> replace)
        {
            yield return GeneralChange<T>.OnRemove(replace.ReplacedItem);
            yield return GeneralChange<T>.OnAdd(replace.Item);
        }

        [NotNull]
        public static IEnumerable<GeneralChange<T>> ToGeneralChanges<T>(
            [NotNull] this IListMoveOperation<T> move)
        {
            yield break;
        }

        [NotNull]
        public static IEnumerable<GeneralChange<T>> ToGeneralChanges<T>(
            [NotNull] this IListClearOperation<T> clear)
        {
            yield return GeneralChange<T>.OnClear();
        }
    }
}
