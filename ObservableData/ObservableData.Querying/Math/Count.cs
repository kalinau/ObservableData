//using System;
//using JetBrains.Annotations;

//namespace ObservableData.Querying.Math
//{
//    internal static class Count
//    {
//        public sealed class Obsever<T> :
//            IObserver<ICollectionChange<T>>
//        {
//            [NotNull] private readonly IObserver<int> _adaptee;

//            private int? _count;

//            public Obsever(
//                [NotNull] IObserver<int> adaptee)
//            {
//                _adaptee = adaptee;
//            }

//            public void OnCompleted() => _adaptee.OnCompleted();

//            public void OnError(Exception error) => _adaptee.OnError(error);

//            public void OnNext(ICollectionChange<T> change)
//            {
//                if (change == null) return;

//                var before = _count;
//                change.Match(
//                    state => _count = state?.Count ?? 0,
//                    delta =>
//                    {
//                        switch (delta.Type)
//                        {
//                            case GeneralChangeType.Add:
//                                _count++;
//                                break;

//                            case GeneralChangeType.Remove:
//                                _count--;
//                                break;

//                            case GeneralChangeType.Clear:
//                                _count = 0;
//                                break;

//                            default:
//                                throw new ArgumentOutOfRangeException();
//                        }
//                    }
//                );
//                if (_count != null && _count != before)
//                {
//                    _adaptee.OnNext(_count.Value);
//                }
//            }
//        }
//    }
//}
