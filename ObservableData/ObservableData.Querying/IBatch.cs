using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using ObservableData.Querying.Compatibility;

namespace ObservableData.Querying
{
    public interface INextValue<out TState, out TChange>
    {
        void Match(Action<TState> onState, Action<TChange> onChange);

        T Match<T>([NotNull] Func<TState, T> onState, [NotNull] Func<TChange, T> onChange);
    }

    public interface IGeneralNextValue<T> : INextValue<IReadOnlyCollection<T>, IBatch<GeneralChange<T>>>
    {
    }

    //public interface IGeneralChange<T> : IGeneralNextValue<T>, IBatch<GeneralChange<T>>
    //{
    //}

    //public interface IGeneralState<T> : IGeneralNextValue<T>, IReadOnlyCollection<T>
    //{
    //}

    public interface IIndexedNextValue<T> : 
        INextValue<IReadOnlyList<T>, IBatch<IndexedChange<T>>>, 
        IGeneralNextValue<T>
    {
    }

    //public interface IIndexedChange<T> : 
    //    IIndexedNextValue<T>, 
    //    IBatch<IndexedChange<T>>, 
    //    IGeneralChange<T>
    //{
    //}

    //public interface IIndexedState<T> : IIndexedNextValue<T>, IReadOnlyList<T>
    //{
    //}

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