using ObservableData.Querying;

namespace ObservableData.Structures.Lists
{
    // ReSharper disable once PossibleInterfaceMemberAmbiguity
    public interface IListChange<T> :
        IChange<ListOperation<T>>,
        IChange<CollectionOperation<T>>,
        IChange<IListOperation<T>>,
        IChange<ICollectionOperation<T>>
    {
    }
}