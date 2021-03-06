using System.Diagnostics.CodeAnalysis;
using ObservableData.Querying;

namespace ObservableData.Structures.Lists
{
    [SuppressMessage("ReSharper", "PossibleInterfaceMemberAmbiguity")]
    public interface IListBatchChange<T> :
        IBatch<IndexedChange<T>>,
        IBatch<GeneralChange<T>>,
        IBatch<IListOperation<T>>,
        IBatch<ICollectionOperation<T>>
    {
    }
}