using JetBrains.Annotations;

namespace ObservableData.Structures.Lists.Utils
{
    internal interface IListBatchChangeNode<T> : IListBatchChange<T>
    {
        [CanBeNull]
        IListBatchChangeNode<T> Next { get; set; }
    }
}