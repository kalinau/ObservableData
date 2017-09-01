//using System;
//using System.Reactive.Disposables;
//using System.Reactive.Linq;
//using JetBrains.Annotations;
//using ObservableData.Querying.Criterions;
//using ObservableData.Querying.Utils;

//namespace ObservableData.Querying
//{
//    public static partial class ObservableExtensions
//    {
//        [NotNull]
//        public static IObservable<bool> AllItems<T>(
//            [NotNull] this IObservable<ICollectionChange<T>> previous,
//            [NotNull] Func<T, bool> criterion)
//        {
//            return Observable.Create<bool>(observer =>
//            {
//                if (observer == null) return Disposable.Empty;

//                var adapter = new All.Observer<T>(observer, criterion);
//                return previous.Subscribe(adapter);
//            }).NotNull();
//        }
//    }
//}