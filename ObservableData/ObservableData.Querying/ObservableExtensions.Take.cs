using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using JetBrains.Annotations;
using ObservableData.Querying.Criterions;
using ObservableData.Querying.Subsets;
using ObservableData.Querying.Utils;

namespace ObservableData.Querying
{
    public static partial class ObservableExtensions
    {
        [NotNull]
        public static IObservableIndexedData<T> TakeItems<T>(
            [NotNull] this IObservableIndexedData<T> previous,
            int limit)
        {
            return Observable.Create<T>(observer =>
            {
                if (observer == null) return Disposable.Empty;

                var adapter = new Take.Observer<T>(null, limit);
                //return previous.Subscribe(adapter);
                return null;
            }).NotNull();
        }

    }
}