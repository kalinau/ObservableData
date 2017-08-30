using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using JetBrains.Annotations;
using ObservableData.Querying.Utils;

namespace ObservableData.Querying.Compatibility
{
    [PublicAPI]
    public sealed class BindableProxy<T> :
        ListProxy<T>,
        INotifyCollectionChanged,
        INotifyPropertyChanged
    {
        [NotNull] private readonly NotifyCollectionEvents<T> _events;

        public BindableProxy()
        {
            _events = new NotifyCollectionEvents<T>();
        }

        public BindableProxy([NotNull] NotifyCollectionEvents<T> events)
        {
            _events = events;
        }

        public BindableProxy([NotNull] IReadOnlyList<T> underlyingList)
            :this()
        {
            this.UnderlyingList = underlyingList;
        }

        public BindableProxy(
            [NotNull] IReadOnlyList<T> underlyingList, 
            [NotNull] NotifyCollectionEvents<T> events)
            : this(events)
        {
            this.UnderlyingList = underlyingList;
        }

        [NotNull]
        public NotifyCollectionEvents<T> Events => _events;

        event NotifyCollectionChangedEventHandler INotifyCollectionChanged.CollectionChanged
        {
            add => _events.CollectionChanged += value;
            remove => _events.CollectionChanged -= value;
        }

        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add => _events.PropertyChanged += value;
            remove => _events.PropertyChanged -= value;
        }
    }
}