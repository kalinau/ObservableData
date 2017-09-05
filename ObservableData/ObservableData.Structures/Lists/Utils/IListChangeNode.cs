using JetBrains.Annotations;
using ObservableData.Querying;

namespace ObservableData.Structures.Lists.Utils
{
    internal interface IListBatchChangeNode<T> : IListChange<T>
    {
        [CanBeNull]
        IListBatchChangeNode<T> Next { get; set; }
    }
}