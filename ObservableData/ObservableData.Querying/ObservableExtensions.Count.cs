//using System;
//using System.Reactive.Disposables;
//using System.Reactive.Linq;
//using JetBrains.Annotations;
//using ObservableData.Querying.Math;
//using ObservableData.Querying.Utils;

//namespace ObservableData.Querying
//{
//    public static partial class ObservableExtensions
//    {
//        [NotNull]
//        public static IObservable<int> CountItems<T>(
//            [NotNull] this IObservable<ICollectionChange<T>> previous)
//        {
//            return Observable.Create<int>(observer =>
//            {
//                if (observer == null) return Disposable.Empty;

//                var adapter = new Count.Obsever<T>(observer);
//                return previous.Subscribe(adapter);
//            }).NotNull();
//        }
//    }
//}