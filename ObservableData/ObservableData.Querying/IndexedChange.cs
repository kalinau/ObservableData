//using System;
//using System.Collections.Generic;
//using JetBrains.Annotations;

//namespace ObservableData.Querying
//{
//    public struct IndexedChange<T>
//    {
//        private IndexedChange(IndexedChangeType type, T item, T changedItem, int index, int originalIndex)
//        {
//            this.Type = type;
//            this.Item = item;
//            this.ChangedItem = changedItem;
//            this.Index = index;
//            this.OriginalIndex = originalIndex;

//        }

//        public static IndexedChange<T> OnClear()
//        {
//            return new IndexedChange<T>(IndexedChangeType.Clear, default(T), default(T), default(int), default(int));
//        }

//        public static IndexedChange<T> OnAdd(T item, int index)
//        {
//            return new IndexedChange<T>(IndexedChangeType.Add, item, default(T), index, default(int));
//        }

//        public static IndexedChange<T> OnRemove(T item, int index)
//        {
//            return new IndexedChange<T>(IndexedChangeType.Remove, item, default(T), index, default(int));
//        }

//        public static IndexedChange<T> OnMove(T item, int index, int originalIndex)
//        {
//            return new IndexedChange<T>(IndexedChangeType.Move, item, default(T), index, originalIndex);
//        }

//        public static IndexedChange<T> OnReplace(T item, T changedItem, int index)
//        {
//            return new IndexedChange<T>(IndexedChangeType.Replace, item, changedItem, index, default(int));
//        }

//        public IndexedChangeType Type { get; }

//        [CanBeNull]
//        public T Item { get; }

//        [CanBeNull]
//        public T ChangedItem { get; }

//        public int Index { get; }

//        public int OriginalIndex { get; }
//    }

//    [PublicAPI]
//    public static class IndexedChangeExtensions
//    {
//        [NotNull]
//        public static IEnumerable<GeneralChange<T>> ToGeneralChanges<T>(this IndexedChange<T> change)
//        {
//            switch (change.Type)
//            {
//                case IndexedChangeType.Add:
//                    yield return GeneralChange<T>.OnAdd(change.Item);
//                    break;

//                case IndexedChangeType.Remove:
//                    yield return GeneralChange<T>.OnRemove(change.Item);
//                    break;

//                case IndexedChangeType.Move:
//                    break;

//                case IndexedChangeType.Replace:
//                    yield return GeneralChange<T>.OnRemove(change.ChangedItem);
//                    yield return GeneralChange<T>.OnAdd(change.Item);
//                    break;

//                case IndexedChangeType.Clear:
//                    yield return GeneralChange<T>.OnClear();
//                    break;

//                default:
//                    throw new ArgumentOutOfRangeException();
//            }
//        }

//        public static void ApplyTo<T>(this IndexedChange<T> change, [NotNull] IList<T> list)
//        {
//            switch (change.Type)
//            {
//                case IndexedChangeType.Add:
//                    list.Insert(change.Index, change.Item);
//                    break;

//                case IndexedChangeType.Remove:
//                    list.RemoveAt(change.Index);
//                    break;

//                case IndexedChangeType.Move:
//                    list.RemoveAt(change.OriginalIndex);
//                    list.Insert(change.Index, change.Item);
//                    break;

//                case IndexedChangeType.Replace:
//                    list[change.Index] = change.Item;
//                    break;

//                case IndexedChangeType.Clear:
//                    list.Clear();
//                    break;

//                default:
//                    throw new ArgumentOutOfRangeException();
//            }
//        }

//        public static void ApplyTo<T>(this IndexedChange<T> change, [NotNull] ICollection<T> collection)
//        {
//            switch (change.Type)
//            {
//                case IndexedChangeType.Add:
//                    collection.Add(change.Item);
//                    break;

//                case IndexedChangeType.Remove:
//                    collection.Remove(change.Item);
//                    break;

//                case IndexedChangeType.Move:
//                    break;

//                case IndexedChangeType.Replace:
//                    collection.Remove(change.ChangedItem);
//                    collection.Add(change.Item);
//                    break;

//                case IndexedChangeType.Clear:
//                    collection.Clear();
//                    break;

//                default:
//                    throw new ArgumentOutOfRangeException();
//            }
//        }

//        public static void Apply<T>([NotNull] this IList<T> list, IndexedChange<T> change)
//        {
//            change.ApplyTo(list);
//        }

//        public static void Apply<T>([NotNull] this ICollection<T> collection, IndexedChange<T> change)
//        {
//            change.ApplyTo(collection);
//        }
//    }
//}