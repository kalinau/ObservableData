using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace ObservableData.Querying.Math
{
    internal static class Count
    {
        public sealed class Obsever<T> :
            IObserver<ICollectionChange<T>>,
            ICollectionChangeEnumerator<T>
        {
            [NotNull] private readonly IObserver<int> _adaptee;

            private int? _count;

            public Obsever([NotNull] IObserver<int> adaptee)
            {
                _adaptee = adaptee;
            }

            void IObserver<ICollectionChange<T>>.OnCompleted() => _adaptee.OnCompleted();

            void IObserver<ICollectionChange<T>>.OnError(Exception error) => _adaptee.OnError(error);

            void IObserver<ICollectionChange<T>>.OnNext(ICollectionChange<T> change)
            {
                if (change == null) return;


                var before = _count;
                change.Enumerate(this);
                if (_count != null && _count != before)
                {
                    _adaptee.OnNext(_count.Value);
                }
            }

            void ICollectionChangeEnumerator<T>.OnStateChanged(IReadOnlyCollection<T> state) => 
                _count = state.Count;

            void ICollectionChangeEnumerator<T>.OnClear() => _count = 0;

            void ICollectionChangeEnumerator<T>.OnAdd(T item) => _count++;

            void ICollectionChangeEnumerator<T>.OnRemove(T item) => _count--;
        }
    }
}
