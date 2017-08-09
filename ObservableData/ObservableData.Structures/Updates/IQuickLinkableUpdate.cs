using ObservableData.Querying.Core;

namespace ObservableData.Structures.Updates
{
    internal interface IQuickLinkableUpdate<T> : IUpdate<T>
    {
        bool IsSingle { get; }

        T First { get; }

        IQuickLinkableUpdate<T> Next { get; set; }
    }
}
