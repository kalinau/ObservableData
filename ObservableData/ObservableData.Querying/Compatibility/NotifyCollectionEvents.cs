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
        public bool HasObservers => this.CollectionChanged != null || this.PropertyChanged != null;

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnChange(IndexedChange<T> value)
        {
            switch (value.Type)
            {
                case IndexedChangeType.Add:
                    this.OnAdd(value);
                    break;

                case IndexedChangeType.Remove:
                    this.OnRemove(value);
                    break;

                case IndexedChangeType.Move:
                    this.OnMove(value);
                    break;

                case IndexedChangeType.Replace:
                    this.OnReplace(value);
                    break;

                case IndexedChangeType.Clear:
                    this.OnClear();
                    return;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OnMove(IndexedChange<T> update)
        {
            var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, update.Item, update.Index, update.OriginalIndex);
            this.CollectionChanged?.Invoke(this, args);
        }

        private void OnRemove(IndexedChange<T> update)
        {
            var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, update.Item, update.Index);
            this.CollectionChanged?.Invoke(this, args);
            this.OnPropertyChanged(nameof(IReadOnlyCollection<T>.Count));
        }

        private void OnAdd(IndexedChange<T> update)
        {
            var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, update.Item, update.Index);
            this.CollectionChanged?.Invoke(this, args);
            this.OnPropertyChanged(nameof(IReadOnlyCollection<T>.Count));
        }

        private void OnReplace(IndexedChange<T> update)
        {
            var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, update.Item, update.ChangedItem, update.Index);
            this.CollectionChanged?.Invoke(this, args);
        }

        private void OnClear()
        {
            this.CollectionChanged?.Invoke(
                this,
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            this.OnPropertyChanged(nameof(IReadOnlyCollection<T>.Count));
        }

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}