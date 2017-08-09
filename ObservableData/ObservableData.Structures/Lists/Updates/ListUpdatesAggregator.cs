using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using JetBrains.Annotations;

namespace ObservableData.Structures.Lists.Updates
{
    public class ListUpdatesAggregator<T> : IObservable<IChange<IListOperation<T>>>, IObservable<IChange<ICollectionOperation<T>>>
    {
        [NotNull] private readonly Subject<IListChange<T>> _subject = new Subject<IListChange<T>>();

        [CanBeNull] private ListBatchChange<T> _batch;

        private bool ShouldNotify => _batch != null || _subject.HasObservers;

        [NotNull]
        public IDisposable StartBatchUpdate()
        {
            if (_batch != null) throw new NotImplementedException("recusive batch updates are not implemented");

            _batch = new ListBatchChange<T>(this.StopBatchUpdate);
            return _batch;
        }

        public IDisposable Subscribe(IObserver<IChange<IListOperation<T>>> observer)
        {
            return _subject.Subscribe(observer);
        }

        public IDisposable Subscribe(IObserver<IChange<ICollectionOperation<T>>> observer)
        {
            return _subject.Subscribe(observer);
        }

        private void StopBatchUpdate(ListBatchChange<T> batch)
        {
            if (batch != _batch) return;
            _batch = null;

            _subject.OnNext(batch);
        }

        public void OnAdd([NotNull] IReadOnlyCollection<T> items, int index)
        {
            if (this.ShouldNotify)
            {
                var update = new ListInsertBatchOperation<T>(items, index);
                this.OnNext(update);
            }
        }

        public void OnAdd(T item, int index)
        {
            if (this.ShouldNotify)
            {
                var update = new ListInsertItemOperation<T>(item, index);
                this.OnNext(update);
            }
        }

        public void OnReset(IReadOnlyCollection<T> items)
        {
            _batch?.Clear();
            if (this.ShouldNotify)
            {
                var update = new ListResetOperation<T>(items);
                this.OnNext(update);
            }
        }

        public void OnMove(T item, int from, int to)
        {
            if (this.ShouldNotify)
            {
                var update = new ListMoveOperation<T>(item, from, to);
                this.OnNext(update);
            }
        }

        public void OnRemove(T item, int index)
        {
            if (this.ShouldNotify)
            {
                var update = new ListRemoveOperation<T>(item, index);
                this.OnNext(update);
            }
        }

        public void OnReplace(T value, T changedItem, int index)
        {
            if (this.ShouldNotify)
            {
                var update = new ListReplaceOperation<T>(index, value, changedItem);
                this.OnNext(update);
            }
        }

        private void OnNext(ListBaseOperation<T> update)
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

    }
}
