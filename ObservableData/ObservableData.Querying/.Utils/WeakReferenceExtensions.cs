using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace ObservableData.Querying.Utils
{
    [PublicAPI]
    public static class WeakReferenceExtensions
    {
        [CanBeNull]
        public static T TryGetTarget<T>([NotNull] this WeakReference<T> reference) where T : class
        {
            reference.TryGetTarget(out var o);
            return o;
        }
    }


    public class ReassignableCollectionChange<T> : ICollectionChange<T>, ICollectionChangeEnumerator<T>
    {
        [NotNull] private readonly ICollectionChangeMap<T> _map;

        private ThreadId? _thread;
        private ICollectionChange<T> _adaptee;
        private ICollectionChangeEnumerator<T> _enumerator;

        public ReassignableCollectionChange([NotNull] ICollectionChangeMap<T> map)
        {
            _map = map;
        }

        void ICollectionChange<T>.Enumerate(ICollectionChangeEnumerator<T> enumerator)
        {
            var change = _adaptee.Check(_thread);
            _enumerator = enumerator;
            change.Enumerate(this);
        }

        void ICollectionChangeEnumerator<T>.OnStateChanged(IReadOnlyCollection<T> state) =>
            _map.RouteStateChanged(_enumerator.Check(_thread), state);

        void ICollectionChangeEnumerator<T>.OnClear() =>
            _map.RouteClear(_enumerator.Check(_thread));

        void ICollectionChangeEnumerator<T>.OnAdd(T item) =>
            _map.RouteAdd(_enumerator.Check(_thread), item);

        void ICollectionChangeEnumerator<T>.OnRemove(T item) =>
            _map.RouteRemove(_enumerator.Check(_thread), item);

        public void Assign(ICollectionChange<T> value)
        {
            _thread = ThreadId.FromCurrent();
            _adaptee = value;
        }

        public void Invalidate()
        {
            _thread = null;
        }
    }

    public class ReassignableCollectionChange<TIn, TOut> : 
        ICollectionChange<TOut>, 
        ICollectionChangeEnumerator<TIn>
    {
        [NotNull] private readonly ICollectionChangeMap<TIn, TOut> _map;

        private ThreadId? _thread;
        private ICollectionChange<TIn> _adaptee;
        private ICollectionChangeEnumerator<TOut> _enumerator;

        public ReassignableCollectionChange([NotNull] ICollectionChangeMap<TIn, TOut> map)
        {
            _map = map;
        }

        void ICollectionChange<TOut>.Enumerate(ICollectionChangeEnumerator<TOut> enumerator)
        {
            var change = _adaptee.Check(_thread);
            _enumerator = enumerator;
            change.Enumerate(this);
        }

        void ICollectionChangeEnumerator<TIn>.OnStateChanged(IReadOnlyCollection<TIn> state) =>
            _map.RouteStateChanged(_enumerator.Check(_thread), state);

        void ICollectionChangeEnumerator<TIn>.OnClear() =>
            _map.RouteClear(_enumerator.Check(_thread));

        void ICollectionChangeEnumerator<TIn>.OnAdd(TIn item) =>
            _map.RouteAdd(_enumerator.Check(_thread), item);

        void ICollectionChangeEnumerator<TIn>.OnRemove(TIn item) =>
            _map.RouteRemove(_enumerator.Check(_thread), item);

        public void Assign(ICollectionChange<TIn> value)
        {
            _thread = ThreadId.FromCurrent();
            _adaptee = value;
        }

        public void Invalidate()
        {
            _thread = null;
        }
    }

    public interface ICollectionChangeMap<T>
    {
        void RouteStateChanged(
            [NotNull] ICollectionChangeEnumerator<T> enumerator,
            [NotNull] IReadOnlyCollection<T> state);

        void RouteClear([NotNull] ICollectionChangeEnumerator<T> enumerator);

        void RouteAdd([NotNull] ICollectionChangeEnumerator<T> enumerator, T item);

        void RouteRemove([NotNull] ICollectionChangeEnumerator<T> enumerator, T item);
    }

    public interface ICollectionChangeMap<in TIn, out TOut>
    {
        //void RouteStateChanged(
        //    [NotNull] ICollectionChangeEnumerator<TOut> enumerator,
        //    [NotNull] IReadOnlyCollection<TIn> state);

        void RouteClear([NotNull] ICollectionChangeEnumerator<TOut> enumerator);

        void RouteAdd([NotNull] ICollectionChangeEnumerator<TOut> enumerator, TIn item);

        void RouteRemove([NotNull] ICollectionChangeEnumerator<TOut> enumerator, TIn item);
    }

    public class CollectionObserver<T> : IObserver<ICollectionChange<T>>
    {
        [NotNull] private readonly IObserver<ICollectionChange<T>> _adaptee;
        [NotNull] private readonly ReassignableCollectionChange<T> _change;

        public CollectionObserver(
            [NotNull] IObserver<ICollectionChange<T>> adaptee,
            [NotNull] ICollectionChangeMap<T> map)
        {
            _adaptee = adaptee;
            _change = new ReassignableCollectionChange<T>(map);
        }

        void IObserver<ICollectionChange<T>>.OnCompleted() => _adaptee.OnCompleted();

        void IObserver<ICollectionChange<T>>.OnError(Exception error) => _adaptee.OnError(error);

        void IObserver<ICollectionChange<T>>.OnNext(ICollectionChange<T> value)
        {
            if (value == null) return;

            _change.Assign(value);
            _adaptee.OnNext(_change);
            _change.Invalidate();
        }
    }

    public class CollectionObserver<TIn, TOut> : IObserver<ICollectionChange<TIn>>
    {
        [NotNull] private readonly IObserver<ICollectionChange<TOut>> _adaptee;
        [NotNull] private readonly ReassignableCollectionChange<TIn, TOut> _change;

        public CollectionObserver(
            [NotNull] IObserver<ICollectionChange<TOut>> adaptee,
            [NotNull] ICollectionChangeMap<TIn, TOut> map)
        {
            _adaptee = adaptee;
            _change = new ReassignableCollectionChange<TIn, TOut>(map);
        }

        void IObserver<ICollectionChange<TIn>>.OnCompleted() => _adaptee.OnCompleted();

        void IObserver<ICollectionChange<TIn>>.OnError(Exception error) => _adaptee.OnError(error);

        void IObserver<ICollectionChange<TIn>>.OnNext(ICollectionChange<TIn> value)
        {
            if (value == null) return;

            _change.Assign(value);
            _adaptee.OnNext(_change);
            _change.Invalidate();
        }
    }

    public class CollectionObserverWithPreparer<T> : IObserver<ICollectionChange<T>>
    {
        [NotNull] private readonly IObserver<ICollectionChange<T>> _adaptee;
        [NotNull] private readonly ICollectionChangeEnumerator<T> _preparer;
        [NotNull] private readonly ReassignableCollectionChange<T> _change;

        public CollectionObserverWithPreparer(
            [NotNull] IObserver<ICollectionChange<T>> adaptee,
            [NotNull] ICollectionChangeEnumerator<T> preparer,
            [NotNull] ICollectionChangeMap<T> map)
        {
            _adaptee = adaptee;
            _preparer = preparer;
            _change = new ReassignableCollectionChange<T>(map);
        }

        void IObserver<ICollectionChange<T>>.OnCompleted() => _adaptee.OnCompleted();

        void IObserver<ICollectionChange<T>>.OnError(Exception error) => _adaptee.OnError(error);

        void IObserver<ICollectionChange<T>>.OnNext(ICollectionChange<T> value)
        {
            if (value == null) return;

            _change.Assign(value);
            value.Enumerate(_preparer);
            _adaptee.OnNext(_change);
            _change.Invalidate();
        }
    }

}