namespace ObservableData.Structures.Lists.Updates
{
    // ReSharper disable once PossibleInterfaceMemberAmbiguity
    public interface IListChange<out T> : IChange<IListOperation<T>>, IChange<ICollectionOperation<T>>
    {
        
    }
}