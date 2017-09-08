using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using System.Linq;

namespace ObservableData.Querying.Criterions
{
    internal static class All
    {
        public sealed class StateObserver<T> :
            IQueryStateObserver<ICollectionChange<T>, IReadOnlyCollection<T>>,
            ICollectionChangeEnumerator<T>
        {
            [NotNull] private readonly IObserver<bool> _adaptee;
            [NotNull] private readonly Func<T, bool> _criterion;

            private bool? _isAll;

            private int _count;
            private int _satisfyCount;

            public StateObserver(
                [NotNull] IObserver<bool> adaptee,
                [NotNull] Func<T, bool> criterion)
            {
                _adaptee = adaptee;
                _criterion = criterion;
            }

            bool IQueryObserver<ICollectionChange<T>>.IsMultiTimesEnumerator => false;

            void IObserver<ICollectionChange<T>>.OnCompleted() => _adaptee.OnCompleted();

            void IObserver<ICollectionChange<T>>.OnError(Exception error) => _adaptee.OnError(error);

            void IObserver<ICollectionChange<T>>.OnNext(ICollectionChange<T> change)
            {
                if (change == null) return;

                change.Enumerate(this);

                var isAll = _count > 0 && _satisfyCount == _count;
                if (isAll != _isAll)
                {
                    _isAll = isAll;
                    _adaptee.OnNext(isAll);
                }
            }


            public void OnStart(IReadOnlyCollection<T> state)
            {
                if (state != null)
                {
                    _count = state.Count;
                    _satisfyCount = state.Count(_criterion);
                }
            }

            void ICollectionChangeEnumerator<T>.OnClear()
            {
                _count = _satisfyCount = 0;
            }

            void ICollectionChangeEnumerator<T>.OnAdd(T item)
            {
                _count++;
                if (_criterion(item))
                {
                    _satisfyCount++;
                }
            }

            void ICollectionChangeEnumerator<T>.OnRemove(T item)
            {
                _count--;
                if (_criterion(item))
                {
                    _satisfyCount--;
                }
            }
        }
    }
}