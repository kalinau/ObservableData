using System.Diagnostics.CodeAnalysis;
using ObservableData.Querying;

namespace ObservableData.Structures.Lists
{
    [SuppressMessage("ReSharper", "PossibleInterfaceMemberAmbiguity")]
    public interface IListChange<T> :
        IChange<ListOperation<T>>,
        IChange<CollectionOperation<T>>,
        IChange<IListOperation<T>>,
        IChange<ICollectionOperation<T>>
    {
    }
}