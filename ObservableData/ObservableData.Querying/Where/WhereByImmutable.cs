//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using JetBrains.Annotations;

//namespace ObservableData.Querying.Where
//{
//    internal static class WhereByImmutable
//    {
//        public class CollectionObserver<T> : IObserver<ICollectionChange<T>>
//        {
//            [NotNull] private readonly IObserver<ICollectionChange<T>> _adaptee;
//            [NotNull] private readonly Func<T, bool> _criterion;
//            [CanBeNull] private CollectionState<T> _state;


//            public CollectionObserver(
//                [NotNull]  IObserver<ICollectionChange<T>> adaptee,
//                [NotNull] Func<T, bool> criterion)
//            {
//                _adaptee = adaptee;
//                _criterion = criterion;
//            }

//            [NotNull]
//            public Func<T, bool> Criterion => _criterion;

//            public CollectionState<T> State
//            {
//                get => _state;
//                set => _state = value;
//            }


//            public void OnNext(ICollectionChange<T> value)
//            {
//                if (value == null) return;

//                var state = _state;
//                if (state != null)
//                {
//                    value.Match(x => _state = null, state.OnChange);
//                }

//                _adaptee.OnNext(new CollectionChange<T>(value, this));
//            }

//            public void OnCompleted() => _adaptee.OnCompleted();

//            public void OnError(Exception error) => _adaptee.OnError(error);
//        }

//        private class CollectionChange<T> : ICollectionChange<T>
//        {
//            [NotNull] private readonly ICollectionChange<T> _adaptee;
//            [NotNull] private readonly CollectionObserver<T> _observer;

//            public CollectionChange(
//                [NotNull] ICollectionChange<T> adaptee,
//                [NotNull] CollectionObserver<T> observer)
//            {
//                _adaptee = adaptee;
//                _observer = observer;
//            }

//            public void Match(
//                Action<IReadOnlyCollection<TOut>> onStateChanged,
//                Action<GeneralChange<TOut>> onDelta)
//            {
//                _adaptee.Match(
//                    state =>
//                    {
//                        onStateChanged?.Invoke(state == null
//                            ? null
//                            : new CollectionState<T>(state, _observer.Criterion));
//                    },
//                    delta => onDelta?.Invoke(delta.Select(_observer)));
//            }
//        }

//        private static GeneralChange<TOut> Select<TIn, TOut>(
//            this GeneralChange<TIn> change,
//            [NotNull] Func<TIn, TOut> selector)
//        {
//            switch (change.Type)
//            {
//                case GeneralChangeType.Add:
//                    return GeneralChange<TOut>.OnAdd(selector(change.Item));

//                case GeneralChangeType.Remove:
//                    return GeneralChange<TOut>.OnRemove(selector(change.Item));

//                case GeneralChangeType.Clear:
//                    return GeneralChange<TOut>.OnClear();

//                default:
//                    throw new ArgumentOutOfRangeException();
//            }
//        }

//        public sealed class GeneralChangesObserver<T> : IObserver<IBatch<GeneralChange<T>>>
//        {
//            [NotNull] private readonly IObserver<IBatch<GeneralChange<T>>> _adaptee;
//            [NotNull] private readonly Func<T, bool> _criterion;

//            public GeneralChangesObserver(
//                [NotNull] IObserver<IBatch<GeneralChange<T>>> adaptee,
//                [NotNull] Func<T, bool> criterion)
//            {
//                _adaptee = adaptee;
//                _criterion = criterion;
//            }

//            public void OnNext(IBatch<GeneralChange<T>> value)
//            {
//                if (value == null) return;

//                _adaptee.OnNext(new GeneralChanges<T>(value, _criterion));
//            }

//            public void OnCompleted() => _adaptee.OnCompleted();

//            public void OnError(Exception error) => _adaptee.OnError(error);
//        }

//        public sealed class GeneralChangesPlusStateObserver<T> : IObserver<GeneralChangesPlusState<T>>
//        {
//            [NotNull] private readonly IObserver<GeneralChangesPlusState<T>> _adaptee;
//            [NotNull] private readonly Func<T, bool> _criterion;

//            private int? _count;

//            public GeneralChangesPlusStateObserver(
//                [NotNull] IObserver<GeneralChangesPlusState<T>> adaptee,
//                [NotNull] Func<T, bool> criterion)
//            {
//                _adaptee = adaptee;
//                _criterion = criterion;
//            }

//            public void OnNext(GeneralChangesPlusState<T> value)
//            {
//                if (_count == null)
//                {
//                    _count = value.ReachedState.Where(_criterion).Count();
//                }
//                else
//                {
//                    foreach (var peace in value.Changes.GetPeaces())
//                    {
//                        switch (peace.Type)
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
//                }
//                var change = new GeneralChanges<T>(value.Changes, _criterion);
//                var state = new CollectionState<T>(value.ReachedState, _count.Value, _criterion);
//                _adaptee.OnNext(new GeneralChangesPlusState<T>(change, state));
//            }

//            public void OnCompleted() => _adaptee.OnCompleted();

//            public void OnError(Exception error) => _adaptee.OnError(error);
//        }

//        private sealed class GeneralChanges<T> : IBatch<GeneralChange<T>>
//        {
//            [NotNull] private readonly IBatch<GeneralChange<T>> _adaptee;
//            [NotNull] private readonly Func<T, bool> _criterion;

//            public GeneralChanges(
//                [NotNull] IBatch<GeneralChange<T>> adaptee,
//                [NotNull] Func<T, bool> criterion)
//            {
//                _adaptee = adaptee;
//                _criterion = criterion;
//            }

//            public IEnumerable<GeneralChange<T>> GetPeaces()
//            {
//                foreach (var update in _adaptee.GetPeaces())
//                {
//                    if (update.Type == GeneralChangeType.Clear)
//                    {
//                        yield return update;
//                    }
//                    else if (_criterion.Invoke(update.Item))
//                    {
//                        yield return update;
//                    }
//                }
//            }

//            public void MakeImmutable() => _adaptee.MakeImmutable();
//        }

//        private sealed class CollectionState<T> : IReadOnlyCollection<T>
//        {
//            [NotNull] private readonly IReadOnlyCollection<T> _source;
//            [NotNull] private readonly Func<T, bool> _criterion;

//            private int? _count;

//            public CollectionState(
//                [NotNull] IReadOnlyCollection<T> source,
//                int count,
//                [NotNull] Func<T, bool> criterion)
//            {
//                _source = source;
//                _criterion = criterion;
//                _count = count;
//            }

//            public int Count
//            {
//                get
//                {
//                    if (_count == null)
//                    {
//                        _count = _source.Count(_criterion);
//                    }
//                    return _count.Value;
//                }
//            }

//            public void OnChange(GeneralChange<T> delta)
//            {
//                switch (delta.Type)
//                {
//                    case GeneralChangeType.Add:
//                        if (_count != null) _count++;
//                        break;

//                    case GeneralChangeType.Remove:
//                        if (_count != null) _count--;
//                        break;

//                    case GeneralChangeType.Clear:
//                        _count = 0;
//                        break;

//                    default:
//                        throw new ArgumentOutOfRangeException();
//                }
//            }

//            public IEnumerator<T> GetEnumerator()
//            {
//                return _source.Where(_criterion).GetEnumerator();
//            }

//            IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
//        }
//    }
//}