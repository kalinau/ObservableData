using System.Collections.Specialized;
using System.ComponentModel;
using JetBrains.Annotations;

namespace ObservableData.Querying.Compatibility
{
    [PublicAPI]
    public sealed class BindableList<T> :
        ListProxy<T>,
        INotifyCollectionChanged,
        INotifyPropertyChanged
    {
        [NotNull] private readonly NotifyCollectionEvents<T> _events = new NotifyCollectionEvents<T>();

        public void OnNext(ListChangePlusState<T> value)
        {
            base.Subject = value.ReachedState;
            foreach (var update in value.Change.GetIterations())
            {
                _events.OnOperation(update);
            }
        }

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