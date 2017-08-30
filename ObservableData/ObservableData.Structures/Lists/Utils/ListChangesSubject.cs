using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
using JetBrains.Annotations;
using ObservableData.Structures.Lists.Operations;
using ObservableData.Structures.Utils;

namespace ObservableData.Structures.Lists.Utils
{
    internal class ListChangesSubject<T> : IObservable<IListBatchChange<T>>
    {
        [NotNull] private readonly Subject<IListBatchChange<T>> _subject = new Subject<IListBatchChange<T>>();
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
                current.Add(new InsertBatchOperation<T>(list, 0));
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
                var update = new InsertBatchOperation<T>(items, index);
                this.OnNext(update);
            }
        }

        public void OnAdd(T item, int index)
        {
            if (this.ShouldTrackChange)
            {
                var update = new InsertItemOperation<T>(index, item);
                this.OnNext(update);
            }
        }

        public void OnRemove(T item, int index)
        {
            if (this.ShouldTrackChange)
            {
                var update = new RemoveItemOperation<T>(index, item);
                this.OnNext(update);
            }
        }

        public void OnMove(T item, int from, int to)
        {
            if (this.ShouldTrackChange)
            {
                var update = new MoveOperation<T>(item, from, to);
                this.OnNext(update);
            }
        }

        public void OnReplace(T value, T changedItem, int index)
        {
            if (this.ShouldTrackChange)
            {
                var update = new ReplaceOperation<T>(index, value, changedItem);
                this.OnNext(update);
            }
        }

        public void OnClear([NotNull] IReadOnlyList<T> state)
        {
            if (_batch != null)
            {
                if (!_batch.IsReadOnly)
                {
                    _batch?.Clear();
                    _batch.Add(ClearOperation<T>.Instance);
                    _batch.Add(new InsertBatchOperation<T>(state, 0));
                    _batch.IsReadOnly = true;
                }
            }
            else if (this.ShouldTrackChange)
            {
                this.OnNext(ClearOperation<T>.Instance);
            }
        }

        private void OnNext(IListBatchChangeNode<T> update)
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

        public IDisposable Subscribe(IObserver<IListBatchChange<T>> observer)
        {
            return _subject.Subscribe(observer);
        }
    }
}