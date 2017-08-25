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
        private readonly ListOperation<T> _adaptee;

        public QueryingOperationAdapter(ListOperation<T> adaptee)
        {
            _adaptee = adaptee;
        }

        IListChangeNode<T> IListChangeNode<T>.Next { get; set; }

        public void MakeImmutable() { }

        IEnumerable<IListOperation<T>> IChange<IListOperation<T>>.GetIterations()
        {
            yield return this;
        }

        IEnumerable<ICollectionOperation<T>> IChange<ICollectionOperation<T>>.GetIterations()
        {
            switch (_adaptee.Type)
            {
                case ListOperationType.Add:
                    yield return this;
                    break;

                case ListOperationType.Remove:
                    yield return this;
                    break;

                case ListOperationType.Move:
                    break;

                case ListOperationType.Replace:
                    yield return new CollectionRemoveOperation {Item = _adaptee.Item};
                    yield return new CollectionInsertOperation(this);
                    break;

                case ListOperationType.Clear:
                    yield return this;
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        IEnumerable<CollectionOperation<T>> IChange<CollectionOperation<T>>.GetIterations()
        {
            return _adaptee.AsForCollection();
        }

        IEnumerable<ListOperation<T>> IChange<ListOperation<T>>.GetIterations()
        {
            yield return _adaptee;
        }

        TResult IListOperation<T>.Match<TResult>(
            Func<IListInsertOperation<T>, TResult> onInsert, 
            Func<IListRemoveOperation<T>, TResult> onRemove, 
            Func<IListReplaceOperation<T>, TResult> onReplace, 
            Func<IListMoveOperation<T>, TResult> onMove, 
            Func<IListClearOperation<T>, TResult> onReset)
        {
            switch (_adaptee.Type)
            {
                case ListOperationType.Add:
                    return onInsert(this);

                case ListOperationType.Remove:
                    return onRemove(this);

                case ListOperationType.Move:
                    return onMove(this);

                case ListOperationType.Replace:
                    return onReplace(this);

                case ListOperationType.Clear:
                    return onReset(this);

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        void IListOperation<T>.Match(
            Action<IListInsertOperation<T>> onInsert, 
            Action<IListRemoveOperation<T>> onRemove, 
            Action<IListReplaceOperation<T>> onReplace, 
            Action<IListMoveOperation<T>> onMove, 
            Action<IListClearOperation<T>> onReset)
        {
            switch (_adaptee.Type)
            {
                case ListOperationType.Add:
                    onInsert?.Invoke(this);
                    break;

                case ListOperationType.Remove:
                    onRemove?.Invoke(this);
                    break;

                case ListOperationType.Move:
                    onMove?.Invoke(this);
                    break;

                case ListOperationType.Replace:
                    onReplace?.Invoke(this);
                    break;

                case ListOperationType.Clear:
                    onReset?.Invoke(this);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public TResult Match<TResult>(
            Func<ICollectionInsertOperation<T>, TResult> onInsert, 
            Func<ICollectionRemoveOperation<T>, TResult> onRemove,
            Func<ICollectionReplaceOperation<T>, TResult> onReplace, 
            Func<ICollectionClearOperation<T>, TResult> onReset)
        {
            switch (_adaptee.Type)
            {
                case ListOperationType.Add:
                    return onInsert(this);

                case ListOperationType.Remove:
                    return onRemove(this);

                case ListOperationType.Clear:
                    return onReset(this);

                case ListOperationType.Replace:
                case ListOperationType.Move:
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void Match(
            Action<ICollectionInsertOperation<T>> onInsert, 
            Action<ICollectionRemoveOperation<T>> onRemove, 
            Action<ICollectionReplaceOperation<T>> onReplace,
            Action<ICollectionClearOperation<T>> onReset)
        {
            switch (_adaptee.Type)
            {
                case ListOperationType.Add:
                    onInsert?.Invoke(this);
                    break;

                case ListOperationType.Remove:
                    onRemove?.Invoke(this);
                    break;

                case ListOperationType.Clear:
                    onReset?.Invoke(this);
                    break;

                case ListOperationType.Replace:
                case ListOperationType.Move:
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        int IListInsertOperation<T>.Index => this.Get(_adaptee.Index, ListOperationType.Add);

        IReadOnlyCollection<T> IListInsertOperation<T>.Items => this.Get(this, ListOperationType.Add);

        int IListRemoveOperation<T>.Index => this.Get(_adaptee.Index, ListOperationType.Remove);

        T IListRemoveOperation<T>.Item => this.Get(_adaptee.Item, ListOperationType.Remove);

        int IListMoveOperation<T>.From => this.Get(_adaptee.OriginalIndex, ListOperationType.Move);

        int IListMoveOperation<T>.To => this.Get(_adaptee.Index, ListOperationType.Move);

        T IListMoveOperation<T>.Item => this.Get(_adaptee.Item, ListOperationType.Move);

        int IListReplaceOperation<T>.Index => this.Get(_adaptee.Index, ListOperationType.Replace);

        T IListReplaceOperation<T>.ReplacedItem => this.Get(_adaptee.ChangedItem, ListOperationType.Replace);

        T IListReplaceOperation<T>.Item => this.Get(_adaptee.Item, ListOperationType.Replace);

        IReadOnlyCollection<T> ICollectionInsertOperation<T>.Items => this.Get(this, ListOperationType.Add);

        T ICollectionRemoveOperation<T>.Item => this.Get(_adaptee.Item, ListOperationType.Remove);

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

            public TResult Match<TResult>(Func<ICollectionInsertOperation<T>, TResult> onInsert, Func<ICollectionRemoveOperation<T>, TResult> onRemove, Func<ICollectionReplaceOperation<T>, TResult> onReplace, Func<ICollectionClearOperation<T>, TResult> onReset)
            {
                return onRemove(this);
            }

            public void Match(Action<ICollectionInsertOperation<T>> onInsert, Action<ICollectionRemoveOperation<T>> onRemove, Action<ICollectionReplaceOperation<T>> onReplace, Action<ICollectionClearOperation<T>> onReset)
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

            public TResult Match<TResult>(Func<ICollectionInsertOperation<T>, TResult> onInsert, Func<ICollectionRemoveOperation<T>, TResult> onRemove, Func<ICollectionReplaceOperation<T>, TResult> onReplace, Func<ICollectionClearOperation<T>, TResult> onReset)
            {
                return onInsert(this);
            }

            public void Match(Action<ICollectionInsertOperation<T>> onInsert, Action<ICollectionRemoveOperation<T>> onRemove, Action<ICollectionReplaceOperation<T>> onReplace, Action<ICollectionClearOperation<T>> onReset)
            {
                onInsert?.Invoke(this);
            }
        }

        [Conditional("DEBUG")]
        private void CheckType(ListOperationType type)
        {
            if (_adaptee.Type != type)
            {
                throw new InvalidOperationException();
            }
        }

        [AssertionMethod]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [ContractAnnotation("value:null=>null; value:notnull=>notnull")]
        private TValue Get<TValue>(TValue value, ListOperationType type)
        {
            this.CheckType(type);
            return value;
        }
    }
}