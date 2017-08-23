using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using ObservableData.Querying;
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
            [NotNull] Func<IListResetOperation<T>, TResult> onReset);

        void Match(
            Action<IListInsertOperation<T>> onInsert,
            Action<IListRemoveOperation<T>> onRemove,
            Action<IListReplaceOperation<T>> onReplace,
            Action<IListMoveOperation<T>> onMove,
            Action<IListResetOperation<T>> onReset);
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
    public interface IListResetOperation<out T> : IListOperation<T>
    {
        [CanBeNull]
        IReadOnlyCollection<T> Items { get; }
    }

    [PublicAPI]
    public static class ListOperationExtensions
    {
        [NotNull]
        public static IEnumerable<ListOperation<T>> AsForListQuerying<T>(
            [NotNull] this IListOperation<T> operation)
        {
            return operation.Match(
                AsForListQuerying,
                AsForListQuerying,
                AsForListQuerying,
                AsForListQuerying,
                AsForListQuerying
            ).NotNull();
        }

        [NotNull]
        public static IEnumerable<ListOperation<T>> AsForListQuerying<T>(
            [NotNull] this IListInsertOperation<T> insert)
        {
            var index = insert.Index;
            foreach (var item in insert.Items)
            {
                yield return ListOperation<T>.OnAdd(item, index++);
            }
        }

        [NotNull]
        public static IEnumerable<ListOperation<T>> AsForListQuerying<T>(
            [NotNull] this IListRemoveOperation<T> remove)
        {
            yield return ListOperation<T>.OnRemove(remove.Item, remove.Index);
        }

        [NotNull]
        public static IEnumerable<ListOperation<T>> AsForListQuerying<T>(
            [NotNull] this IListReplaceOperation<T> replace)
        {
            yield return ListOperation<T>.OnReplace(replace.Item, replace.ReplacedItem, replace.Index);
        }

        [NotNull]
        public static IEnumerable<ListOperation<T>> AsForListQuerying<T>(
            [NotNull] this IListMoveOperation<T> move)
        {
            yield return ListOperation<T>.OnMove(move.Item, move.To, move.From);
        }

        [NotNull]
        public static IEnumerable<ListOperation<T>> AsForListQuerying<T>(
            [NotNull] this IListResetOperation<T> reset)
        {
            yield return ListOperation<T>.OnClear();
            var index = 0;
            if (reset.Items != null)
            {
                foreach (var item in reset.Items)
                {
                    yield return ListOperation<T>.OnAdd(item, index++);
                }
            }
        }

        [NotNull]
        public static IEnumerable<CollectionOperation<T>> AsForCollectionQuerying<T>(
            [NotNull] this IListOperation<T> operation)
        {
            return operation.Match(
                AsForCollectionQuerying,
                AsForCollectionQuerying,
                AsForCollectionQuerying,
                AsForCollectionQuerying,
                AsForCollectionQuerying
            ).NotNull();
        }

        [NotNull]
        public static IEnumerable<CollectionOperation<T>> AsForCollectionQuerying<T>(
            [NotNull] this IListInsertOperation<T> insert)
        {
            foreach (var item in insert.Items)
            {
                yield return CollectionOperation<T>.OnAdd(item);
            }
        }

        [NotNull]
        public static IEnumerable<CollectionOperation<T>> AsForCollectionQuerying<T>(
            [NotNull] this IListRemoveOperation<T> remove)
        {
            yield return CollectionOperation<T>.OnRemove(remove.Item);
        }

        [NotNull]
        public static IEnumerable<CollectionOperation<T>> AsForCollectionQuerying<T>(
            [NotNull] this IListReplaceOperation<T> replace)
        {
            yield return CollectionOperation<T>.OnRemove(replace.ReplacedItem);
            yield return CollectionOperation<T>.OnAdd(replace.Item);
        }

        [NotNull]
        public static IEnumerable<CollectionOperation<T>> AsForCollectionQuerying<T>(
            [NotNull] this IListMoveOperation<T> move)
        {
            return Enumerable.Empty<CollectionOperation<T>>().NotNull();
        }

        [NotNull]
        public static IEnumerable<CollectionOperation<T>> AsForCollectionQuerying<T>(
            [NotNull] this IListResetOperation<T> reset)
        {
            yield return CollectionOperation<T>.OnClear();
            if (reset.Items != null)
            {
                foreach (var item in reset.Items)
                {
                    yield return CollectionOperation<T>.OnAdd(item);
                }
            }
        }
    }
}
