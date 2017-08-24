using System;
using JetBrains.Annotations;
using ObservableData.Querying.Compatibility;
using ObservableData.Querying.Utils;

namespace ObservableData.Querying
{
    public static partial class ObservableExtensions
    {
        [NotNull]
        public static IDisposable ToBindableStateProxy<T>(
            [NotNull] this IObservable<ChangedListData<T>> observable,
            [NotNull] out BindableList<T> state)
        {
            var list = new BindableList<T>();
            state = list;
            return observable.Subscribe(x => list.OnNext(x)).NotNull();
        }
    }
}