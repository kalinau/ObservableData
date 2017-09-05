using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using ObservableData.Querying.Utils;

namespace ObservableData.Querying.Where
{
    internal static class WhereByImmutable
    {
        public class CollectionObserver<T> : 
            IObserver<ICollectionChange<T>>,
            ICollectionChange<T>,
            ICollectionChangeEnumerator<T>
        {
            [NotNull] private readonly IObserver<ICollectionChange<T>> _adaptee;
            [NotNull] private readonly Func<T, bool> _criterion;
            [NotNull] private readonly CollectionState<T> _state;

            private ThreadId? _thread;
            private ICollectionChange<T> _change;
            private ICollectionChangeEnumerator<T> _enumerator;

            public CollectionObserver(
                [NotNull]  IObserver<ICollectionChange<T>> adaptee,
                [NotNull] Func<T, bool> criterion)
            {
                _adaptee = adaptee;
                _criterion = criterion;
                _state = new CollectionState<T>(_criterion);
            }

            void IObserver<ICollectionChange<T>>.OnCompleted() => _adaptee.OnCompleted();

            void IObserver<ICollectionChange<T>>.OnError(Exception error) => _adaptee.OnError(error);

            void IObserver<ICollectionChange<T>>.OnNext(ICollectionChange<T> value)
            {
                if (value == null) return;

                value.Enumerate(_state);

                _thread = ThreadId.FromCurrent();
                _change = value;
                _adaptee.OnNext(this);
                _thread = null;
            }

            void ICollectionChange<T>.Enumerate(ICollectionChangeEnumerator<T> enumerator)
            {
                var change = _change.Check(_thread);
                _enumerator = enumerator;
                change.Enumerate(this);
            }

            void ICollectionChangeEnumerator<T>.OnStateChanged(IReadOnlyCollection<T> state)
            {
                _enumerator.Check(_thread).OnStateChanged(_state);
                _enumerator = null;
            }

            void ICollectionChangeEnumerator<T>.OnClear()
            {
                _enumerator.Check(_thread).OnClear();
            }

            void ICollectionChangeEnumerator<T>.OnAdd(T item)
            {
                if (_criterion(item))
                {
                    _enumerator.Check(_thread).OnAdd(item);
                }
            }

            void ICollectionChangeEnumerator<T>.OnRemove(T item)
            {
                if (_criterion(item))
                {
                    _enumerator.Check(_thread).OnRemove(item);
                }
            }
        }

        private sealed class CollectionState<T> : 
            IReadOnlyCollection<T>,
            ICollectionChangeEnumerator<T>
        {
            [NotNull] private IReadOnlyCollection<T> _source = EmptyList<T>.Instance;
            [NotNull] private readonly Func<T, bool> _criterion;

            private int? _count;

            public CollectionState([NotNull] Func<T, bool> criterion)
            {
                _criterion = criterion;
            }

            int IReadOnlyCollection<T>.Count
            {
                get
                {
                    if (_count == null)
                    {
                        _count = _source.Count(_criterion);
                    }
                    return _count.Value;
                }
            }

            public IEnumerator<T> GetEnumerator()
            {
                return _source.Where(_criterion).GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

            void ICollectionChangeEnumerator<T>.OnStateChanged(IReadOnlyCollection<T> state)
            {
                _source = state;
                _count = null;
            }

            void ICollectionChangeEnumerator<T>.OnClear()
            {

                _count = 0;
            }

            void ICollectionChangeEnumerator<T>.OnAdd(T item)
            {
                if (_count != null && _criterion(item))
                {
                    _count++;
                }
            }

            void ICollectionChangeEnumerator<T>.OnRemove(T item)
            {
                if (_count != null && _criterion(item))
                {
                    _count--;
                }
            }
        }
    }
}