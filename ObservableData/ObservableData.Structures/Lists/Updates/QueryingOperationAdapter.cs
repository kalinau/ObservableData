using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using ObservableData.Querying;

namespace ObservableData.Structures.Lists.Updates
{
    public class QueryingOperationAdapter<T> :
        IListChangeNode<T>,

        //it's implement here with the reason to decrement allocation for internal temporary instances
        ICollectionInsertOperation<T>,
        ICollectionRemoveOperation<T>,
        ICollectionClearOperation<T>,

        IListInsertOperation<T>,
        IListRemoveOperation<T>,
        IListReplaceOperation<T>,
        IListMoveOperation<T>,
        IListClearOperation<T>,

        IReadOnlyCollection<T>
    {
        private readonly IndexedChange<T> _adaptee;

        public QueryingOperationAdapter(IndexedChange<T> adaptee)
        {
            _adaptee = adaptee;
        }

        IListChangeNode<T> IListChangeNode<T>.Next { get; set; }

        public void MakeImmutable() { }

        IEnumerable<IListOperation<T>> IBatch<IListOperation<T>>.GetPeaces()
        {
            yield return this;
        }

        IEnumerable<ICollectionOperation<T>> IBatch<ICollectionOperation<T>>.GetPeaces()
        {
            switch (_adaptee.Type)
            {
                case IndexedChangeType.Add:
                    yield return this;
                    break;

                case IndexedChangeType.Remove:
                    yield return this;
                    break;

                case IndexedChangeType.Move:
                    break;

                case IndexedChangeType.Replace:
                    yield return new CollectionRemoveOperation {Item = _adaptee.Item};
                    yield return new CollectionInsertOperation(this);
                    break;

                case IndexedChangeType.Clear:
                    yield return this;
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        IEnumerable<GeneralChange<T>> IBatch<GeneralChange<T>>.GetPeaces()
        {
            return _adaptee.ToGeneralChanges();
        }

        IEnumerable<IndexedChange<T>> IBatch<IndexedChange<T>>.GetPeaces()
        {
            yield return _adaptee;
        }

        TResult IListOperation<T>.Match<TResult>(
            Func<IListInsertOperation<T>, TResult> onInsert, 
            Func<IListRemoveOperation<T>, TResult> onRemove, 
            Func<IListReplaceOperation<T>, TResult> onReplace, 
            Func<IListMoveOperation<T>, TResult> onMove, 
            Func<IListClearOperation<T>, TResult> onClear)
        {
            switch (_adaptee.Type)
            {
                case IndexedChangeType.Add:
                    return onInsert(this);

                case IndexedChangeType.Remove:
                    return onRemove(this);

                case IndexedChangeType.Move:
                    return onMove(this);

                case IndexedChangeType.Replace:
                    return onReplace(this);

                case IndexedChangeType.Clear:
                    return onClear(this);

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        void IListOperation<T>.Match(
            Action<IListInsertOperation<T>> onInsert, 
            Action<IListRemoveOperation<T>> onRemove, 
            Action<IListReplaceOperation<T>> onReplace, 
            Action<IListMoveOperation<T>> onMove, 
            Action<IListClearOperation<T>> onClear)
        {
            switch (_adaptee.Type)
            {
                case IndexedChangeType.Add:
                    onInsert?.Invoke(this);
                    break;

                case IndexedChangeType.Remove:
                    onRemove?.Invoke(this);
                    break;

                case IndexedChangeType.Move:
                    onMove?.Invoke(this);
                    break;

                case IndexedChangeType.Replace:
                    onReplace?.Invoke(this);
                    break;

                case IndexedChangeType.Clear:
                    onClear?.Invoke(this);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public TResult Match<TResult>(
            Func<ICollectionInsertOperation<T>, TResult> onInsert, 
            Func<ICollectionRemoveOperation<T>, TResult> onRemove,
            Func<ICollectionReplaceOperation<T>, TResult> onReplace, 
            Func<ICollectionClearOperation<T>, TResult> onClear)
        {
            switch (_adaptee.Type)
            {
                case IndexedChangeType.Add:
                    return onInsert(this);

                case IndexedChangeType.Remove:
                    return onRemove(this);

                case IndexedChangeType.Clear:
                    return onClear(this);

                case IndexedChangeType.Replace:
                case IndexedChangeType.Move:
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void Match(
            Action<ICollectionInsertOperation<T>> onInsert, 
            Action<ICollectionRemoveOperation<T>> onRemove, 
            Action<ICollectionReplaceOperation<T>> onReplace,
            Action<ICollectionClearOperation<T>> onClear)
        {
            switch (_adaptee.Type)
            {
                case IndexedChangeType.Add:
                    onInsert?.Invoke(this);
                    break;

                case IndexedChangeType.Remove:
                    onRemove?.Invoke(this);
                    break;

                case IndexedChangeType.Clear:
                    onClear?.Invoke(this);
                    break;

                case IndexedChangeType.Replace:
                case IndexedChangeType.Move:
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        int IListInsertOperation<T>.Index => this.Get(_adaptee.Index, IndexedChangeType.Add);

        IReadOnlyCollection<T> IListInsertOperation<T>.Items => this.Get(this, IndexedChangeType.Add);

        int IListRemoveOperation<T>.Index => this.Get(_adaptee.Index, IndexedChangeType.Remove);

        T IListRemoveOperation<T>.Item => this.Get(_adaptee.Item, IndexedChangeType.Remove);

        int IListMoveOperation<T>.From => this.Get(_adaptee.OriginalIndex, IndexedChangeType.Move);

        int IListMoveOperation<T>.To => this.Get(_adaptee.Index, IndexedChangeType.Move);

        T IListMoveOperation<T>.Item => this.Get(_adaptee.Item, IndexedChangeType.Move);

        int IListReplaceOperation<T>.Index => this.Get(_adaptee.Index, IndexedChangeType.Replace);

        T IListReplaceOperation<T>.ReplacedItem => this.Get(_adaptee.ChangedItem, IndexedChangeType.Replace);

        T IListReplaceOperation<T>.Item => this.Get(_adaptee.Item, IndexedChangeType.Replace);

        IReadOnlyCollection<T> ICollectionInsertOperation<T>.Items => this.Get(this, IndexedChangeType.Add);

        T ICollectionRemoveOperation<T>.Item => this.Get(_adaptee.Item, IndexedChangeType.Remove);

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            yield return _adaptee.Item;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            yield return _adaptee.Item;
        }

        int IReadOnlyCollection<T>.Count => 1;

        private sealed class CollectionRemoveOperation : ICollectionRemoveOperation<T>
        {
            public T Item { get; set; }

            public TResult Match<TResult>(Func<ICollectionInsertOperation<T>, TResult> onInsert, Func<ICollectionRemoveOperation<T>, TResult> onRemove, Func<ICollectionReplaceOperation<T>, TResult> onReplace, Func<ICollectionClearOperation<T>, TResult> onClear)
            {
                return onRemove(this);
            }

            public void Match(Action<ICollectionInsertOperation<T>> onInsert, Action<ICollectionRemoveOperation<T>> onRemove, Action<ICollectionReplaceOperation<T>> onReplace, Action<ICollectionClearOperation<T>> onClear)
            {
                onRemove?.Invoke(this);
            }
        }

        private sealed class CollectionInsertOperation : ICollectionInsertOperation<T>
        {
            [NotNull]
            private IReadOnlyCollection<T> _items;

            public CollectionInsertOperation([NotNull] IReadOnlyCollection<T> items)
            {
                _items = items;
            }

            public IReadOnlyCollection<T> Items => _items;

            public TResult Match<TResult>(Func<ICollectionInsertOperation<T>, TResult> onInsert, Func<ICollectionRemoveOperation<T>, TResult> onRemove, Func<ICollectionReplaceOperation<T>, TResult> onReplace, Func<ICollectionClearOperation<T>, TResult> onClear)
            {
                return onInsert(this);
            }

            public void Match(Action<ICollectionInsertOperation<T>> onInsert, Action<ICollectionRemoveOperation<T>> onRemove, Action<ICollectionReplaceOperation<T>> onReplace, Action<ICollectionClearOperation<T>> onClear)
            {
                onInsert?.Invoke(this);
            }
        }

        [Conditional("DEBUG")]
        private void CheckType(IndexedChangeType type)
        {
            if (_adaptee.Type != type)
            {
                throw new InvalidOperationException();
            }
        }

        [AssertionMethod]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [ContractAnnotation("value:null=>null; value:notnull=>notnull")]
        private TValue Get<TValue>(TValue value, IndexedChangeType type)
        {
            this.CheckType(type);
            return value;
        }
    }
}