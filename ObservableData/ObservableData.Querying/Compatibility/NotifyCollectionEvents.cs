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
        INotifyPropertyChanged
    {
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnCompleted() {}

        public void OnError(Exception error) {}

        public void OnOperation(ListOperation<T> value)
        {
            switch (value.Type)
            {
                case ListOperationType.Add:
                    this.OnAdd(value);
                    break;

                case ListOperationType.Remove:
                    this.OnRemove(value);
                    break;

                case ListOperationType.Move:
                    this.OnMove(value);
                    break;

                case ListOperationType.Replace:
                    this.OnReplace(value);
                    break;

                case ListOperationType.Clear:
                    this.OnClear();
                    return;

                default:
                    throw new ArgumentOutOfRangeException();
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