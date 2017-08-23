using JetBrains.Annotations;

namespace ObservableData.Structures.Lists.Updates
{
    internal interface IListChangeNode<T> : IListChange<T>
    {
        [CanBeNull]
        IListChangeNode<T> Next { get; set; }
    }
}