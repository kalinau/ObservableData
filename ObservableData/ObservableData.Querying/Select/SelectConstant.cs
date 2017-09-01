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
            ITrickyEnumerator<GeneralChange<TIn>>
            //IReadOnlyCollection<TOut>
        {
            [NotNull] private readonly IObserver<ICollectionChange<TOut>> _adaptee;
            [NotNull] private readonly Func<TIn, TOut> _selector;

            private CollectionState<TIn, TOut> _state;

            private ThreadId? _currentChangeThread;
            private ICollectionChange<TIn> _currentChange;
            private ITrickyEnumerator<GeneralChange<TOut>> _currentEnumerator;

            public CollectionObserver(
                [NotNull] IObserver<ICollectionChange<TOut>> adaptee,
                [NotNull] Func<TIn, TOut> selector)
            {
                _adaptee = adaptee;
                _selector = selector;
            }

            void IObserver<ICollectionChange<TIn>>.OnCompleted() => _adaptee.OnCompleted();

            void IObserver<ICollectionChange<TIn>>.OnError(Exception error) => _adaptee.OnError(error);

            void IObserver<ICollectionChange<TIn>>.OnNext(ICollectionChange<TIn> value)
            {
                if (value == null) return;

                _currentChangeThread = ThreadId.FromCurrent();
                _currentChange = value;

                var state = value.State;
                if (!ReferenceEquals(_state?.Source, state))
                {
                    _state = state == null
                        ? null
                        : new CollectionState<TIn, TOut>(state, _selector);
                }
                _adaptee.OnNext(this);

                _currentChangeThread = null;
            }

            void ITrickyEnumerable<GeneralChange<TOut>>.Enumerate(ITrickyEnumerator<GeneralChange<TOut>> enumerator)
            {
                var change = _currentChange.GetSafety(_currentChangeThread);
                _currentEnumerator = enumerator;
                change.Enumerate(this);
            }

            bool ITrickyEnumerator<GeneralChange<TIn>>.OnNext(GeneralChange<TIn> item)
            {
                var enumerator = _currentEnumerator.GetSafety(_currentChangeThread);
                return enumerator.OnNext(item.Select(_selector));
            }

            public IReadOnlyCollection<TOut> State => _state.GetSafety(_currentChangeThread);

            //public int Count => _state.GetSafety().Count;

            //public IEnumerator<TOut> GetEnumerator() => _state.GetSafety().Select(_selector).GetEnumerator();

            //IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
            //public IReadOnlyCollection<TOut> State { get; }
            //public IReadOnlyCollection<TOut> State { get; }
        }

        //public class ListObserver<TIn, TOut> :
        //  IObserver<ICollectionChange<TIn>>,
        //  IListChange<TOut>,
        //  ITrickyEnumerable<GeneralChange<TOut>>,
        //  IReadOnlyList<TOut>
        //{
        //    [NotNull] private readonly Func<IndexedChange<TIn>, bool> _onChange;
        //    [NotNull] private readonly IObserver<IListChange<TOut>> _adaptee;
        //    [NotNull] private readonly Func<TIn, TOut> _selector;

        //    private IReadOnlyList<TIn> _state;

        //    private ThreadId? _currentChangeThread;
        //    private IListChange<TIn> _currentChange;
        //    private Func<IndexedChange<TOut>, bool> _currentEnumerator;

        //    public ListObserver(
        //        [NotNull] IObserver<IListChange<TOut>> adaptee,
        //        [NotNull] Func<TIn, TOut> selector)
        //    {
        //        _adaptee = adaptee;
        //        _selector = selector;
        //        _onChange = this.OnChange;
        //    }

        //    void IObserver<ICollectionChange<TIn>>.OnCompleted() => _adaptee.OnCompleted();

        //    void IObserver<ICollectionChange<TIn>>.OnError(Exception error) => _adaptee.OnError(error);

        //    void IObserver<ICollectionChange<TIn>>.OnNext(ICollectionChange<TIn> value)
        //    {
        //        if (value == null) return;

        //        _currentChangeThread = ThreadId.FromCurrent();

        //        _currentChange = value;
        //        _state = value.TryGetState() ?? _state;
        //        _adaptee.OnNext(this);

        //        _currentChangeThread = null;
        //    }


        //    public ITrickyEnumerable<IndexedChange<TOut>> TryGetDelta()
        //    {
        //        var change = _currentChange.GetSafety(_currentChangeThread);

        //        var delta = change.TryGetDelta();
        //        if (delta != null)
        //        {
        //            return this;
        //        }
        //        return null;
        //    }

        //    public IReadOnlyList<TOut> TryGetState()
        //    {
        //        var change = _currentChange.GetSafety(_currentChangeThread);

        //        var state = change.TryGetState();
        //        if (state != null)
        //        {
        //            return this;
        //        }
        //        return null;
        //    }

        //    IReadOnlyCollection<TOut> ICollectionChange<TOut>.TryGetState() => this.TryGetState();

        //    ITrickyEnumerable<GeneralChange<TOut>> ICollectionChange<TOut>.TryGetDelta()

        //    void ITrickyEnumerable<GeneralChange<TOut>>.Enumerate(Func<GeneralChange<TOut>, bool> handle)
        //    {
        //        _currentChangeThread.CheckIsCurrent();

        //        _currentEnumerator = handle;
        //        _currentChange?.TryGetDelta()?.Enumerate(_onChange);
        //    }

        //    private bool OnChange(IndexedChange<TIn> change)
        //    {
        //        var enumerator = _currentEnumerator.GetSafety(_currentChangeThread);
        //        return enumerator.Invoke(change.Select(_selector));
        //    }

        //    public int Count => _state.GetSafety().Count;

        //    public IEnumerator<TOut> GetEnumerator() => _state.GetSafety().Select(_selector).GetEnumerator();

        //    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        //    public TOut this[int index] => _selector(_state.GetSafety()[index]);
        //}


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

        private class CollectionState<TIn, TOut> : IReadOnlyCollection<TOut>
        {
            [NotNull] private readonly IReadOnlyCollection<TIn> _source;
            [NotNull] private readonly Func<TIn, TOut> _selector;

            public CollectionState(
                [NotNull] IReadOnlyCollection<TIn> source,
                [NotNull] Func<TIn, TOut> selector)
            {
                _source = source;
                _selector = selector;
            }

            public IReadOnlyCollection<TIn> Source => _source;

            public int Count => _source.Count;

            public IEnumerator<TOut> GetEnumerator() => _source.Select(_selector).GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
        }

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