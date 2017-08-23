using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace ObservableData.Querying.Compatibility
{
    [PublicAPI]
    public sealed class NotifyCollectionEvents<T> :
        INotifyCollectionChanged,
        INotifyPropertyChanged,
        IObserver<IChange<ListOperation<T>>>
    {
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnCompleted() {}

        public void OnError(Exception error) {}

        public void OnNext([NotNull] IChange<ListOperation<T>> value)
        {
            foreach (var update in value.GetIterations())
            {
                switch (update.Type)
                {
                    case ListOperationType.Add:
                        this.OnAdd(update);
                        break;

                    case ListOperationType.Remove:
                        this.OnRemove(update);
                        break;

                    case ListOperationType.Move:
                        this.OnMove(update);
                        break;

                    case ListOperationType.Replace:
                        this.OnReplace(update);
                        break;

                    case ListOperationType.Clear:
                        this.OnClear();
                        return;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private void OnMove(ListOperation<T> update)
        {
            var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, update.Item, update.Index, update.OriginalIndex);
            this.CollectionChanged?.Invoke(this, args);
        }

        private void OnRemove(ListOperation<T> update)
        {
            var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, update.Item, update.Index);
            this.CollectionChanged?.Invoke(this, args);
            this.OnPropertyChanged(nameof(IReadOnlyCollection<T>.Count));
        }

        private void OnAdd(ListOperation<T> update)
        {
            var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, update.Item, update.Index);
            this.CollectionChanged?.Invoke(this, args);
            this.OnPropertyChanged(nameof(IReadOnlyCollection<T>.Count));
        }

        private void OnReplace(ListOperation<T> update)
        {
            var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, update.Item, update.ChangedItem, update.Index);
            this.CollectionChanged?.Invoke(this, args);
        }

        private void OnClear()
        {
            this.CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            this.OnPropertyChanged(nameof(IReadOnlyCollection<T>.Count));
        }

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}