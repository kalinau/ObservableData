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

        public void OnReset()
        {
            this.CollectionChanged?.Invoke(
                this,
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            this.OnPropertyChanged(nameof(IReadOnlyCollection<T>.Count));
        }

        public void OnAdd(T item, int index)
        {
            var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index);
            this.CollectionChanged?.Invoke(this, args);
            this.OnPropertyChanged(nameof(IReadOnlyCollection<T>.Count));
        }

        public void OnRemove(T item, int index)
        {
            var args = new NotifyCollectionChangedEventArgs(
                NotifyCollectionChangedAction.Remove,
                item,
                index);
            this.CollectionChanged?.Invoke(this, args);
            this.OnPropertyChanged(nameof(IReadOnlyCollection<T>.Count));
        }

        public void OnMove(T item, int index, int originalIndex)
        {
            var args = new NotifyCollectionChangedEventArgs(
                NotifyCollectionChangedAction.Move,
                item, 
                index, 
                originalIndex);
            this.CollectionChanged?.Invoke(this, args);
        }

        public void OnReplace(T item, T changedItem, int index)
        {
            var args = new NotifyCollectionChangedEventArgs(
                NotifyCollectionChangedAction.Replace,
                item,
                changedItem,
                index);
            this.CollectionChanged?.Invoke(this, args);
        }

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}