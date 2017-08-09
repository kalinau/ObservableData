using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace ObservableData.Structures
{
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

    public interface IListInsertOperation<out T> : IListOperation<T>
    {
        int Index { get; }

        [NotNull]
        IReadOnlyCollection<T> Items { get; }
    }

    public interface IListRemoveOperation<out T> : IListOperation<T>
    {
        int Index { get; }

        T Item { get; }
    }

    public interface IListReplaceOperation<out T> : IListOperation<T>
    {
        int Index { get; }

        T Item { get; }

        T ReplacedItem { get; }
    }

    public interface IListMoveOperation<out T> : IListOperation<T>
    {
        T Item { get; }

        int From { get; }

        int To { get; }
    }

    public interface IListResetOperation<out T> : IListOperation<T>
    {
        [CanBeNull]
        IReadOnlyCollection<T> Items { get; }
    }
}
