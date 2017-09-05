using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace ObservableData.Querying
{
    public struct GeneralChange<T>
    {
        private GeneralChange(GeneralChangeType type, T item)
        {
            this.Item = item;
            this.Type = type;
        }

        [CanBeNull]
        public T Item { get; }

        public GeneralChangeType Type { get; }

        public static GeneralChange<T> OnClear()
        {
            return new GeneralChange<T>(GeneralChangeType.Clear, default(T));
        }

        public static GeneralChange<T> OnAdd(T item)
        {
            return new GeneralChange<T>(GeneralChangeType.Add, item);
        }

        public static GeneralChange<T> OnRemove(T item)
        {
            return new GeneralChange<T>(GeneralChangeType.Remove, item);
        }
    }

    //[PublicAPI]
    //public static class GeneralChangeExtensions
    //{
    //    public static void ApplyTo<T>(
    //        this GeneralChange<T> change,
    //        [NotNull] ICollection<T> list)
    //    {
    //        switch (change.Type)
    //        {
    //            case GeneralChangeType.Add:
    //                list.Add(change.Item);
    //                break;

    //            case GeneralChangeType.Remove:
    //                list.Remove(change.Item);
    //                break;

    //            case GeneralChangeType.Clear:
    //                list.Clear();
    //                break;

    //            default:
    //                throw new ArgumentOutOfRangeException();
    //        }
    //    }
    //}
}