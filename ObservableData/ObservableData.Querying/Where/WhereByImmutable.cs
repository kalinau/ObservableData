using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using ObservableData.Querying.Utils.Adapters;
using ObservableData.Structures;

namespace ObservableData.Querying.Where
{
    internal static class WhereByImmutable
    {
        public sealed class CollectionOperationsObserver<T> 
            : ObserverAdapter<IChange<CollectionOperation<T>>>
        {
            [NotNull] private readonly Func<T, bool> _criterion;

            public CollectionOperationsObserver(
                [NotNull] IObserver<IChange<CollectionOperation<T>>> previous,
                [NotNull] Func<T, bool> criterion) 
                : base(previous)
            {
                _criterion = criterion;
            }

            public override void OnNext(IChange<CollectionOperation<T>> value)
            {
                if (value == null) return;

                this.Adaptee.OnNext(new Change(value, _criterion));
            }


            private sealed class Change : IChange<CollectionOperation<T>>
            {
                [NotNull] private readonly IChange<CollectionOperation<T>> _adaptee;
                [NotNull] private readonly Func<T, bool> _criterion;

                public Change(
                    [NotNull] IChange<CollectionOperation<T>> adaptee, 
                    [NotNull] Func<T, bool> criterion)
                {
                    _adaptee = adaptee;
                    _criterion = criterion;
                }
                public void Lock() => _adaptee.Lock();

                public IEnumerable<CollectionOperation<T>> Operations()
                {
                    foreach (var update in _adaptee.Operations())
                    {
                        if (update.Type == CollectionOperationType.Clear)
                        {
                            yield return update;
                        }
                        else if (_criterion.Invoke(update.Item))
                        {
                            yield return update;
                        }
                    }
                }
            }
        }
    }
}