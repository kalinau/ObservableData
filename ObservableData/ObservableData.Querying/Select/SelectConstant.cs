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


        public sealed class ListEnumerator<TIn, TOut> : IListChangeEnumerator<TIn>
        {
            private sealed class State : IReadOnlyList<TOut>
            {
                [NotNull] private IReadOnlyList<TIn> _source;
                [NotNull] private readonly Func<TIn, TOut> _selector;

                public State([NotNull] Func<TIn, TOut> selector, [NotNull] IReadOnlyList<TIn> source)
                {
                    _selector = selector;
                    _source = source;
                }

                public void ChangeSource([NotNull] IReadOnlyList<TIn> source)
                {
                    _source = source;
                }

                public TOut this[int index] => _selector(_source[index]);

                public int Count => _source.Count;

                public IEnumerator<TOut> GetEnumerator() => _source.Select(_selector).GetEnumerator();

                IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
            }

            [NotNull] private readonly IObserver<IListChange<TOut>> _adaptee;
            [NotNull] private readonly Func<TIn, TOut> _selector;

            [NotNull] private readonly State _state;

            private ThreadId? _thread;
            private IListChange<TIn> _currentChange;
            private IListChangeEnumerator<TOut> _enumerator;

            private ListToCollectionChangeEnumerator<TOut> _collectionEnumeratorBuffer;

            public ListEnumerator(
                [NotNull] IObserver<IListChange<TOut>> adaptee,
                [NotNull] Func<TIn, TOut> selector)
            {
                _adaptee = adaptee;
                _selector = selector;
                _state = new State(selector);
            }

            void IObserver<IListChange<TIn>>.OnCompleted() => _adaptee.OnCompleted();

            void IObserver<IListChange<TIn>>.OnError(Exception error) => _adaptee.OnError(error);

            void IObserver<IListChange<TIn>>.OnNext(IListChange<TIn> value)
            {
                if (value == null) return;

                _thread = ThreadId.FromCurrent();

                _currentChange = value;
                _adaptee.OnNext(this);

                _thread = null;
            }

            void ICollectionChange<TOut>.Enumerate(ICollectionChangeEnumerator<TOut> enumerator)
            {
                var change = _currentChange.Check(_thread);
                _enumerator = enumerator.FromBuffer(ref _collectionEnumeratorBuffer);
                change.Enumerate(this);
            }

            void IListChange<TOut>.Enumerate(IListChangeEnumerator<TOut> enumerator)
            {
                var change = _currentChange.Check(_thread);
                _enumerator = enumerator;
                change.Enumerate(this);
            }

            void IListChangeEnumerator<TIn>.OnStateChanged(IReadOnlyList<TIn> state)
            {
                var enumerator = _enumerator.Check(_thread);
                _state.ChangeSource(state);
                enumerator.OnStateChanged(_state);
                _enumerator = null;
            }

            void IListChangeEnumerator<TIn>.OnClear() =>
                _enumerator.Check(_thread).OnClear();

            void IListChangeEnumerator<TIn>.OnAdd(TIn item, int index) =>
                _enumerator.Check(_thread).OnAdd(_selector(item), index);

            void IListChangeEnumerator<TIn>.OnRemove(TIn item, int index) =>
                _enumerator.Check(_thread).OnRemove(_selector(item), index);

            void IListChangeEnumerator<TIn>.OnMove(TIn item, int index, int originalIndex) =>
                _enumerator.Check(_thread).OnMove(_selector(item), index, originalIndex);

            void IListChangeEnumerator<TIn>.OnReplace(TIn item, TIn changedItem, int index) =>
                _enumerator.Check(_thread).OnReplace(_selector(item), _selector(changedItem), index);
        }

    }
}