using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using ObservableData.Querying.Utils.Adapters;
using ObservableData.Structures;

namespace ObservableData.Querying.Select.Constant
{
    public sealed class SelectConstantQuery<TIn, TOut> : IQuery<TOut>
    {
        [NotNull] private readonly IQuery<TIn> _previous;
        [NotNull] private readonly Func<TIn, TOut> _func;

        public SelectConstantQuery([NotNull] IQuery<TIn> previous, [NotNull] Func<TIn, TOut> func)
        {
            _previous = previous;
            _func = func;
        }

        public void IgnoreEfficiency() { }

        public IDisposable Subscribe(IObserver<IChange<CollectionOperation<TOut>>> observer)
        {
            var adapter = observer.Wrap(
                this.Adapt,
                default(IChange<CollectionOperation<TIn>>));

            return _previous.Subscribe(adapter);
        }
         
        public IDisposable Subscribe(IObserver<IChange<CollectionOperation<TOut>>> observer, out IReadOnlyCollection<TOut> mutableState)
        {
            var adapter = observer.Wrap(
                this.Adapt,
                default(IChange<CollectionOperation<TIn>>));

            IReadOnlyCollection<TIn> previousState;
            var result = _previous.Subscribe(adapter, out previousState);
            mutableState = previousState.SelectForCollection(_func);
            return result;
        }

        public IDisposable Subscribe(IObserver<IChange<ListOperation<TOut>>> observer)
        {
            var adapter = observer.Wrap(
                this.Adapt,
                default(IChange<ListOperation<TIn>>));

            return _previous.Subscribe(adapter);
        }

        public IDisposable Subscribe(IObserver<IChange<ListOperation<TOut>>> observer, out IReadOnlyList<TOut> mutableState)
        {
            var adapter = observer.Wrap(
                this.Adapt,
                default(IChange<ListOperation<TIn>>));

            IReadOnlyList<TIn> previousState;
            var result = _previous.Subscribe(adapter, out previousState);
            mutableState = new SelectConstantList<TIn, TOut>(previousState, _func);
            return result;
        }

        private IChange<ListOperation<TOut>> Adapt([CanBeNull] IChange<ListOperation<TIn>> e)
        {
            if (e == null) return null;

            IEnumerable<ListOperation<TOut>> Enumerate(IEnumerable<ListOperation<TIn>> adaptee)
            {
                if (adaptee == null) yield break;

                foreach (var update in adaptee)
                {
                    switch (update.Type)
                    {
                        case ListOperationType.Add:
                            yield return ListOperation<TOut>.OnAdd(_func(update.Item), update.Index);
                            break;

                        case ListOperationType.Remove:
                            yield return ListOperation<TOut>.OnRemove(_func(update.Item), update.Index);
                            break;

                        case ListOperationType.Move:
                            yield return ListOperation<TOut>.OnMove(_func(update.Item), update.Index, update.OriginalIndex);
                            break;

                        case ListOperationType.Replace:
                            yield return ListOperation<TOut>.OnReplace(_func(update.Item), _func(update.ChangedItem),
                                update.Index);
                            break;

                        case ListOperationType.Clear:
                            yield return ListOperation<TOut>.OnClear();
                            break;

                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }

            return e.Adapt(Enumerate);
        }

        private IChange<CollectionOperation<TOut>> Adapt([CanBeNull] IChange<CollectionOperation<TIn>> e)
        {
            if (e == null) return null;

            IEnumerable<CollectionOperation<TOut>> Enumerate(IEnumerable<CollectionOperation<TIn>> updates)
            {
                if (updates == null) yield break;

                foreach (var u in updates)
                {
                    switch (u.Type)
                    {
                        case CollectionOperationType.Add:
                            yield return CollectionOperation<TOut>.OnAdd(_func(u.Item));
                            break;

                        case CollectionOperationType.Remove:
                            yield return CollectionOperation<TOut>.OnRemove(_func(u.Item));
                            break;

                        case CollectionOperationType.Clear:
                            yield return CollectionOperation<TOut>.OnClear();
                            break;

                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }

            return e.Adapt(Enumerate);
        }
    }
}
