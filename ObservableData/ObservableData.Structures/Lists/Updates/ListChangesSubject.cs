using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
using JetBrains.Annotations;
using ObservableData.Querying;
using ObservableData.Structures.Utils;

namespace ObservableData.Structures.Lists.Updates
{
    internal class ListChangesSubject<T> : IObservable<IListChange<T>>
    {
        [NotNull] private readonly Subject<IListChange<T>> _subject = new Subject<IListChange<T>>();

        [CanBeNull] private ListBatchChange<T> _batch;

        private bool ShouldNotify => _batch != null || _subject.HasObservers;

        [NotNull]
        public IDisposable StartBatchUpdate()
        {
            if (_batch != null)
            {
                throw new NotImplementedException("recusive batch update is not implemented");
            }

            var current = new ListBatchChange<T>();
            _batch = current;
            return Disposable.Create(() =>
            {
                if (_batch != current) return;
                _batch = null;

                _subject.OnNext(current);
            }).NotNull();
        }

        public void OnOperation(ListOperation<T> operation)
        {
            if (operation.Type == ListOperationType.Clear)
            {
                _batch?.Clear();
            }
            if (this.ShouldNotify)
            {
                var update = new QueryingOperationAdapter<T>(operation);
                this.OnNext(update);
            }
        }

        public void OnAddBatch([NotNull] IReadOnlyCollection<T> items, int index)
        {
            if (this.ShouldNotify)
            {
                var update = new ListInsertBatchOperation<T>(items, index);
                this.OnNext(update);
            }
        }

        public void OnReset([CanBeNull] IReadOnlyCollection<T> items)
        {
            _batch?.Clear();
            if (this.ShouldNotify)
            {
                var update = new ListResetOperation<T>(items);
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

        public IDisposable Subscribe(IObserver<IListChange<T>> observer)
        {
            return _subject.Subscribe(observer);
        }
    }
}