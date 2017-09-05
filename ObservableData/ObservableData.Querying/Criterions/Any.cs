using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace ObservableData.Querying.Criterions
{
    internal static class Any
    {
        public sealed class Observer<T> : 
            IObserver<ICollectionChange<T>>,
            ICollectionChangeEnumerator<T>
        {
            [NotNull] private readonly IObserver<bool> _adaptee;
            [NotNull] private readonly Func<T, bool> _criterion;

            private bool? _isAny;
            private int _satisfyCount;

            public Observer(
                [NotNull] IObserver<bool> adaptee,
                [NotNull] Func<T, bool> criterion)
            {
                _adaptee = adaptee;
                _criterion = criterion;
            }

            void IObserver<ICollectionChange<T>>.OnCompleted() => _adaptee.OnCompleted();

            void IObserver<ICollectionChange<T>>.OnError(Exception error) => _adaptee.OnError(error);

            void IObserver<ICollectionChange<T>>.OnNext(ICollectionChange<T> change)
            {
                if (change == null) return;

                change.Enumerate(this);

                var isAny = _satisfyCount > 0;
                if (isAny != _isAny)
                {
                    _isAny = isAny;
                    _adaptee.OnNext(isAny);
                }
            }

            void ICollectionChangeEnumerator<T>.OnStateChanged(IReadOnlyCollection<T> state)
            {
                _satisfyCount = state.Count(_criterion);
            }

            void ICollectionChangeEnumerator<T>.OnClear()
            {
                _satisfyCount = 0;
            }

            void ICollectionChangeEnumerator<T>.OnAdd(T item)
            {
                if (_criterion(item))
                {
                    _satisfyCount++;
                }
            }

            void ICollectionChangeEnumerator<T>.OnRemove(T item)
            {
                if (_criterion(item))
                {
                    _satisfyCount--;
                }
            }
        }
    }
}