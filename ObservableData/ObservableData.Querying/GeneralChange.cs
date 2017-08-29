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

        public GeneralChangeType Type { get; }

        [CanBeNull]
        public T Item { get; }
    }

    [PublicAPI]
    public static class GeneralChangeExtensions
    {
        public static void ApplyTo<T>(
            this GeneralChange<T> change,
            [NotNull] ICollection<T> list)
        {
            switch (change.Type)
            {
                case GeneralChangeType.Add:
                    list.Add(change.Item);
                    break;

                case GeneralChangeType.Remove:
                    list.Remove(change.Item);
                    break;

                case GeneralChangeType.Clear:
                    list.Clear();
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static void ApplyTo<T>(
            [NotNull] this IBatch<GeneralChange<T>> change,
            [NotNull] ICollection<T> collection)
        {
            foreach (var o in change.GetPeaces())
            {
                o.ApplyTo(collection);
            }
        }
    }
}