//using System;
//using JetBrains.Annotations;
//using System.Linq;

//namespace ObservableData.Querying.Criterions
//{
//    internal static class All
//    {
//        public sealed class Observer<T> : IObserver<ICollectionChange<T>>
//        {
//            [NotNull] private readonly IObserver<bool> _adaptee;
//            [NotNull] private readonly Func<T, bool> _criterion;

//            private bool? _state;
//            private int _count;
//            private int _satisfyCount;

//            public Observer(
//                [NotNull] IObserver<bool> adaptee,
//                [NotNull] Func<T, bool> criterion)
//            {
//                _adaptee = adaptee;
//                _criterion = criterion;
//            }

//            public void OnNext(ICollectionChange<T> change)
//            {
//                if (change == null) return;
//                change.Match(s =>
//                {
//                    _count = s?.Count ?? 0;
//                    _satisfyCount = s?.Count(_criterion) ?? 0;
//                },
//                delta =>
//                {
//                    switch (delta.Type)
//                    {
//                        case GeneralChangeType.Add:
//                            _count++;
//                            if (_criterion(delta.Item))
//                            {
//                                _satisfyCount++;
//                            }
//                            break;

//                        case GeneralChangeType.Remove:
//                            _count--;
//                            if (_criterion(delta.Item))
//                            {
//                                _satisfyCount--;
//                            }
//                            break;

//                        case GeneralChangeType.Clear:
//                            _count = _satisfyCount = 0;
//                            break;

//                        default:
//                            throw new ArgumentOutOfRangeException();
//                    }
//                });
//                var state = _satisfyCount == _count;
//                if (state != _state)
//                {
//                    _state = state;
//                    _adaptee.OnNext(state);
//                }
//            }

//            public void OnCompleted() => _adaptee.OnCompleted();

//            public void OnError(Exception error) => _adaptee.OnError(error);
//        }
//    }
//}