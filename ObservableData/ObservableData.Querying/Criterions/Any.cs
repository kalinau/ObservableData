//using System;
//using System.Linq;
//using JetBrains.Annotations;

//namespace ObservableData.Querying.Criterions
//{
//    internal static class Any
//    {
//        public sealed class Observer<TSum> : IObserver<ICollectionChange<TSum>>
//        {
//            [NotNull] private readonly IObserver<bool> _adaptee;
//            [NotNull] private readonly Func<TSum, bool> _criterion;

//            private bool? _state;
//            private int _satisfyCount;

//            public Observer(
//                [NotNull] IObserver<bool> adaptee,
//                [NotNull] Func<TSum, bool> criterion)
//            {
//                _adaptee = adaptee;
//                _criterion = criterion;
//            }

//            public void OnNext(ICollectionChange<TSum> change)
//            {
//                if (change == null) return;

//                change.Match(s =>
//                {
//                    _satisfyCount = s?.Count(_criterion) ?? 0;
//                },
//                delta =>
//                {
//                    switch (delta.Type)
//                    {
//                        case GeneralChangeType.Add:
//                            if (_criterion(delta.Item))
//                            {
//                                _satisfyCount++;
//                            }
//                            break;

//                        case GeneralChangeType.Remove:
//                            if (_criterion(delta.Item))
//                            {
//                                _satisfyCount--;
//                            }
//                            break;

//                        case GeneralChangeType.Clear:
//                             _satisfyCount = 0;
//                            break;

//                        default:
//                            throw new ArgumentOutOfRangeException();
//                    }
//                });
//                var state = _satisfyCount > 0;
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