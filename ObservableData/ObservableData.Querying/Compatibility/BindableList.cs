using System.Collections.Specialized;
using System.ComponentModel;
using JetBrains.Annotations;
using ObservableData.Querying.Utils;

namespace ObservableData.Querying.Compatibility
{
    [PublicAPI]
    public sealed class BindableList<T> :
        ListProxy<T>,
        INotifyCollectionChanged,
        INotifyPropertyChanged
    {
        [NotNull] private readonly NotifyCollectionEvents<T> _events = new NotifyCollectionEvents<T>();

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