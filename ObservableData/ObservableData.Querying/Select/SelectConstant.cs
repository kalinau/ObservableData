using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using ObservableData.Querying.Utils;

namespace ObservableData.Querying.Select
{
    internal static class SelectConstant
    {
        public class CollectionObserver<TIn, TOut> :
          IObserver<ICollectionChange<TIn>>,
          ICollectionChange<TOut>,
          ITrickyEnumerable<GeneralChange<TOut>>,
          IReadOnlyCollection<TOut>
        {
            [NotNull] private readonly Func<GeneralChange<TIn>, bool> _onChange;
            [NotNull] private readonly IObserver<ICollectionChange<TOut>> _adaptee;
            [NotNull] private readonly Func<TIn, TOut> _selector;

            private IReadOnlyCollection<TIn> _state;

            private ThreadId? _currentChangeThread;
            private ICollectionChange<TIn> _currentChange;
            private Func<GeneralChange<TOut>, bool> _currentEnumerator;

            public CollectionObserver(
                [NotNull] IObserver<ICollectionChange<TOut>> adaptee,
                [NotNull] Func<TIn, TOut> selector)
            {
                _adaptee = adaptee;
                _selector = selector;
                _onChange = this.OnChange;
            }

            void IObserver<ICollectionChange<TIn>>.OnCompleted() => _adaptee.OnCompleted();

            void IObserver<ICollectionChange<TIn>>.OnError(Exception error) => _adaptee.OnError(error);

            void IObserver<ICollectionChange<TIn>>.OnNext(ICollectionChange<TIn> value)
            {
                if (value == null) return;

                _currentChangeThread = ThreadId.FromCurrent();

                _currentChange = value;
                _state = value.TryGetState() ?? _state;
                _adaptee.OnNext(this);

                _currentChangeThread = null;
            }

            IReadOnlyCollection<TOut> ICollectionChange<TOut>.TryGetState()
            {
                var change = _currentChange.GetSafety(_currentChangeThread);

                var state = change.TryGetState();
                if (state != null)
                {
                    return this;
                }
                return null;
            }

            ITrickyEnumerable<GeneralChange<TOut>> ICollectionChange<TOut>.TryGetDelta()
            {
                var change = _currentChange.GetSafety(_currentChangeThread);

                var delta = change.TryGetDelta();
                if (delta != null)
                {
                    return this;
                }
                return null;
            }

            void ITrickyEnumerable<GeneralChange<TOut>>.Enumerate(Func<GeneralChange<TOut>, bool> handle)
            {
                _currentChangeThread.CheckIsCurrent();

                _currentEnumerator = handle;
                _currentChange?.TryGetDelta()?.Enumerate(_onChange);
            }

            private bool OnChange(GeneralChange<TIn> change)
            {
                var enumerator = _currentEnumerator.GetSafety(_currentChangeThread);
                return enumerator.Invoke(change.Select(_selector));
            }

            public int Count => _state.GetSafety().Count;

            public IEnumerator<TOut> GetEnumerator() => _state.GetSafety().Select(_selector).GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
        }



        //public class CollectionChange<TIn, TOut> : ICollectionChange<TOut>
        //{
        //    [NotNull] private readonly ICollectionChange<TIn> _adaptee;
        //    [NotNull] private readonly Func<TIn, TOut> _selector;

        //    public CollectionChange(
        //        [NotNull] ICollectionChange<TIn> adaptee,
        //        [NotNull] Func<TIn, TOut> selector)
        //    {
        //        _adaptee = adaptee;
        //        _selector = selector;
        //    }

        //    public void Match(
        //        Action<IReadOnlyCollection<TOut>> onStateChanged,
        //        Action<GeneralChange<TOut>> onDelta)
        //    {
        //        _adaptee.Match(
        //            state =>
        //            {
        //                onStateChanged?.Invoke(state == null
        //                    ? null
        //                    : new CollectionState<TIn, TOut>(state, _selector));
        //            },
        //            delta => onDelta?.Invoke(delta.Select(_selector)));
        //    }
        //}

        //private class CollectionState<TIn, TOut> : IReadOnlyCollection<TOut>
        //{
        //    [NotNull] private readonly IReadOnlyCollection<TIn> _source;
        //    [NotNull] private readonly Func<TIn, TOut> _selector;

        //    public CollectionState(
        //        [NotNull] IReadOnlyCollection<TIn> source,
        //        [NotNull] Func<TIn, TOut> selector)
        //    {
        //        _source = source;
        //        _selector = selector;
        //    }

        //    public int Count => _source.Count;

        //    public IEnumerator<TOut> GetEnumerator() => _source.Select(_selector).GetEnumerator();

        //    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
        //}

        //public sealed class ListChange<TIn, TOut> :IListChange<TOut>
        //{
        //    [NotNull] private readonly IListChange<TIn> _adaptee;
        //    [NotNull] private readonly Func<TIn, TOut> _selector;

        //    public ListChange(
        //        [NotNull] IListChange<TIn> adaptee,
        //        [NotNull] Func<TIn, TOut> selector)
        //    {
        //        _adaptee = adaptee;
        //        _selector = selector;
        //    }

        //    public void Match(
        //        Action<IReadOnlyList<TOut>> onStateChanged,
        //        Action<IndexedChange<TOut>> onIteration)
        //    {
        //        _adaptee.Match(
        //            state =>
        //            {
        //                onStateChanged?.Invoke(state == null
        //                    ? null
        //                    : new ListState<TIn, TOut>(state, _selector));
        //            },
        //            delta => onIteration?.Invoke(delta.Select(_selector)));
        //    }

        //    public void Match(Action<IReadOnlyCollection<TOut>> onStateChanged, Action<GeneralChange<TOut>> onDelta)
        //    {
        //        _adaptee.Match(
        //            state =>
        //            {
        //                onStateChanged?.Invoke(state == null
        //                    ? null
        //                    : new CollectionState<TIn,TOut>(state, _selector));
        //            },
        //            delta => onDelta?.Invoke(delta.Select(_selector)));
        //    }
        //}

        //private sealed class ListState<TIn, TOut> : IReadOnlyList<TOut>
        //{
        //    [NotNull] private readonly IReadOnlyList<TIn> _source;
        //    [NotNull] private readonly Func<TIn, TOut> _selector;

        //    public ListState(
        //        [NotNull] IReadOnlyList<TIn> source,
        //        [NotNull] Func<TIn, TOut> selector)
        //    {
        //        _source = source;
        //        _selector = selector;
        //    }

        //    public TOut this[int index] => _selector(_source[index]);

        //    public int Count => _source.Count;

        //    public IEnumerator<TOut> GetEnumerator() => _source.Select(_selector).GetEnumerator();

        //    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
        //}

        private static GeneralChange<TOut> Select<TIn, TOut>(
            this GeneralChange<TIn> change,
            [NotNull] Func<TIn, TOut> selector)
        {
            switch (change.Type)
            {
                case GeneralChangeType.Add:
                    return GeneralChange<TOut>.OnAdd(selector(change.Item));

                case GeneralChangeType.Remove:
                    return GeneralChange<TOut>.OnRemove(selector(change.Item));

                case GeneralChangeType.Clear:
                    return GeneralChange<TOut>.OnClear();

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static IndexedChange<TOut> Select<TIn, TOut>(
            this IndexedChange<TIn> change,
            [NotNull] Func<TIn, TOut> selector)
        {
            switch (change.Type)
            {
                case IndexedChangeType.Add:
                    return IndexedChange<TOut>.OnAdd(
                        selector(change.Item),
                        change.Index);

                case IndexedChangeType.Remove:
                    return IndexedChange<TOut>.OnRemove(
                        selector(change.Item),
                        change.Index);

                case IndexedChangeType.Move:
                    return IndexedChange<TOut>.OnMove(
                        selector(change.Item),
                        change.Index,
                        change.OriginalIndex);

                case IndexedChangeType.Replace:
                    return IndexedChange<TOut>.OnReplace(
                        selector(change.Item),
                        selector(change.ChangedItem),
                        change.Index);

                case IndexedChangeType.Clear:
                    return IndexedChange<TOut>.OnClear();

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}