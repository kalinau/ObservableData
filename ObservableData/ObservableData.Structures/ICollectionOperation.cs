using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace ObservableData.Structures
{
    [PublicAPI]
    public interface ICollectionOperation<out T>
    {
        TResult Match<TResult>(
            [NotNull] Func<ICollectionInsertOperation<T>, TResult> onInsert,
            [NotNull] Func<ICollectionRemoveOperation<T>, TResult> onRemove,
            [NotNull] Func<ICollectionReplaceOperation<T>, TResult> onReplace,
            [NotNull] Func<ICollectionResetOperation<T>, TResult> onReset);

        void Match(
            Action<ICollectionInsertOperation<T>> onInsert,
            Action<ICollectionRemoveOperation<T>> onRemove,
            Action<ICollectionReplaceOperation<T>> onReplace,
            Action<ICollectionResetOperation<T>> onReset);
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

    public interface ICollectionResetOperation<out T> : ICollectionOperation<T>
    {
        [CanBeNull]
        IReadOnlyCollection<T> Items { get; }
    }
}