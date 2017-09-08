using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using JetBrains.Annotations;
using ObservableData.Querying.Utils;

namespace ObservableData.Querying.Where
{
    public class MutableItemsEnumerator<T> : 
        ICollectionChangeEnumerator<T>,
        ICollectionChangeMap<T>,
        IDisposable
    {
        [NotNull] private readonly WeakReference<MutableItemsEnumerator<T>> _thisWeakReference;
        [NotNull] private readonly Dictionary<T, IItemObserver> _observers = new Dictionary<T, IItemObserver>();
        
        [NotNull] private readonly Func<T, bool> _criterion;
        [NotNull] private readonly Func<T, IObservable<bool>> _whenStateChanged;
        [NotNull] private readonly CollectionState<T> _state;

        public MutableItemsEnumerator(
            [NotNull] Func<T, bool> criterion, 
            [NotNull] Func<T, IObservable<bool>> whenStateChanged)
        {
            _thisWeakReference = new WeakReference<MutableItemsEnumerator<T>>(this);
            _criterion = criterion;
            _whenStateChanged = whenStateChanged;
            _state = new CollectionState<T>(_criterion);
        }

        void ICollectionChangeEnumerator<T>.OnStateChanged(IReadOnlyCollection<T> state)
        {
            _enumerator.Check(_thread).OnStateChanged(_state);
            _enumerator = null;
        }

        void ICollectionChangeEnumerator<T>.OnClear()
        {
            this.Clear();
        }

        void ICollectionChangeEnumerator<T>.OnAdd(T item)
        {
            bool currentState = _criterion(item);

            if (_observers.TryGetValue(item, out var observer))
            {
                observer.IncrementCount();
            }
            else
            {
                var whenChanged = _whenStateChanged(item);
                if (whenChanged == null)
                {
                    observer = new SilentObserver();
                }
                else
                {
                    observer = new ItemObserver(
                        item,
                        currentState,
                        whenChanged,
                        _thisWeakReference,
                        _observers);
                }
                _observers.Add(item, observer);
            }
            if (currentState)
            {
                _state.IncreaseCount();
            }
        }

        private interface IItemObserver : IDisposable
        {
            void IncrementCount();

            bool TryDecreaseCount();
        }


        private sealed class ItemObserver : IObserver<bool>, IItemObserver
        {
            private readonly T _item;
            bool _currentState;

            private int _count = 1;

            [NotNull] private readonly WeakReference<MutableItemsEnumerator<T>> _reference;
            [NotNull] private readonly Dictionary<T, IItemObserver> _itemObservers;
            [CanBeNull] private readonly IDisposable _sub;

            public ItemObserver(
                T item,
                bool currentState,
                [NotNull] IObservable<bool> whenStateChanged,
                [NotNull] WeakReference<MutableItemsEnumerator<T>> reference,
                [NotNull] Dictionary<T, IItemObserver> itemObservers)
            {
                _item = item;
                _currentState = currentState;
                _itemObservers = itemObservers;
                _reference = reference;
                _sub = whenStateChanged.Subscribe(this);
            }

            public void IncrementCount()
            {
                _count++;
            }

            public bool TryDecreaseCount()
            {
                if (_count > 0)
                {
                    _count--;
                    return true;
                }
                return false;
            }

            public void OnCompleted() { }

            public void OnError(Exception error) { }

            public void OnNext(bool state)
            {
                if (_currentState == state) return;
                _currentState = state;

                var observer = _reference.TryGetTarget();
                if (observer == null)
                {
                    foreach (var v in _itemObservers.Values.NotNull())
                    {
                        v?.Dispose();
                    }
                    _itemObservers.Clear();
                }
                else
                {
                    observer.OnStateChanged(_item, _currentState, _count);
                }
            }

            public void Dispose()
            {
                _sub?.Dispose();
            }
        }

        private sealed class SilentObserver : IItemObserver
        {
            private int _count = 1;

            public void Dispose()
            {
            }

            public void IncrementCount() => _count++;

            public bool TryDecreaseCount()
            {
                if (_count > 0)
                {
                    _count--;
                    return true;
                }
                return false;
            }
        }

        private void OnStateChanged(T item, bool currentState, int count)
        {

        }

        void ICollectionChangeEnumerator<T>.OnRemove(T item)
        {

            //if (_criterion(item))
            //{
            //    _enumerator.Check(_thread).OnRemove(item);
            //}
        }

        public void RouteStateChanged(ICollectionChangeEnumerator<T> enumerator, IReadOnlyCollection<T> state)
        {
            throw new NotImplementedException();
        }

        public void RouteClear(ICollectionChangeEnumerator<T> enumerator)
        {
            throw new NotImplementedException();
        }

        public void RouteAdd(ICollectionChangeEnumerator<T> enumerator, T item)
        {
            throw new NotImplementedException();
        }

        public void RouteRemove(ICollectionChangeEnumerator<T> enumerator, T item)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            foreach (var v in _observers.Values.NotNull())
            {
                v?.Dispose();
            }
            _observers.Clear();
        }
    }
}