using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
using JetBrains.Annotations;
using ObservableData.Querying;
using ObservableData.Structures.Utils;

namespace ObservableData.Structures.Lists.Updates
{
    internal class ListChangesSubject<T> : IObservable<IListBatch<T>>
    {
        [NotNull] private readonly Subject<IListBatch<T>> _subject = new Subject<IListBatch<T>>();
        [CanBeNull] private ListBatchChange<T> _batch;

        private bool ShouldTrackChange
        {
            get
            {
                if (_batch == null) return _subject.HasObservers;

                return !_batch.IsReadOnly;
            }
        }

        [NotNull]
        public IDisposable StartBatchUpdate([NotNull] IReadOnlyList<T> list)
        {
            if (_batch != null)
            {
                throw new NotImplementedException("recusive batch update is not implemented");
            }

            var current = new ListBatchChange<T>();

            if (list.Count == 0)
            {
                current.Add(new ListInsertBatchOperation<T>(list, 0));
                current.IsReadOnly = true;
            }

            _batch = current;
            return Disposable.Create(() =>
            {
                if (_batch != current) return;
                _batch = null;

                _subject.OnNext(current);
            }).NotNull();
        }

        public void OnAddBatch([NotNull] IReadOnlyCollection<T> items, int index)
        {
            if (this.ShouldTrackChange)
            {
                var update = new ListInsertBatchOperation<T>(items, index);
                this.OnNext(update);
            }
        }

        public void OnAdd(T item, int index)
        {
            this.OnOperation(IndexedChange<T>.OnAdd(item, index));
        }

        public void OnRemove(T item, int index)
        {
            this.OnOperation(IndexedChange<T>.OnRemove(item, index));
        }

        public void OnMove(T item, int from, int to)
        {
            this.OnOperation(IndexedChange<T>.OnMove(item, to, from));
        }

        public void OnReplace(T value, T changedItem, int index)
        {
            this.OnOperation(IndexedChange<T>.OnReplace(value, changedItem, index));
        }

        public void OnClear([NotNull] IReadOnlyList<T> state)
        {
            if (_batch != null)
            {
                if (!_batch.IsReadOnly)
                {
                    _batch?.Clear();
                    _batch.Add(ListClearOperation<T>.Instance);
                    _batch.Add(new ListInsertBatchOperation<T>(state, 0));
                    _batch.IsReadOnly = true;
                }
            }
            else
            {
                this.OnOperation(IndexedChange<T>.OnClear());
            }
        }

        private void OnOperation(IndexedChange<T> change)
        {
            if (this.ShouldTrackChange)
            {
                var update = new QueryingOperationAdapter<T>(change);
                this.OnNext(update);
            }
        }

        private void OnNext(IListChangeNode<T> update)
        {
            if (_batch != null)
            {
                _batch.Add(update);
            }
            else
            {
                _subject.OnNext(update);
            }
        }

        public IDisposable Subscribe(IObserver<IListBatch<T>> observer)
        {
            return _subject.Subscribe(observer);
        }
    }
}