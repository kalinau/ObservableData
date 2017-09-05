using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using JetBrains.Annotations;
using ObservableData.Querying.Compatibility;
using ObservableData.Querying.Utils;

namespace ObservableData.Querying
{
    public interface IListChange<out T> : ICollectionChange<T>
    {
        void Enumerate([NotNull] IListChangeEnumerator<T> enumerator);
    }


    [PublicAPI]
    public static class ListChangeExtensions
    {
        private sealed class ToList<T> : IListChangeEnumerator<T>
        {
            [NotNull] private static readonly ThreadLocal<ToList<T>> Factory =
                new ThreadLocal<ToList<T>>(() => new ToList<T>());

            private IList<T> _list;

            [NotNull]
            public static ToList<T> GetInstance([NotNull] IList<T> list)
            {
                var result = Factory.Value.NotNull();
                result.SetList(list);
                return result;
            }
            private void SetList(IList<T> list)
            {
                _list = list;
            }

            public void OnStateChanged(IReadOnlyList<T> state)
            {
                throw new System.NotImplementedException();
            }

            public void OnClear() => _list.Check().Clear();

            public void OnAdd(T item, int index) => _list.Check().Insert(index, item);

            public void OnRemove(T item, int index) => _list.Check().RemoveAt(index);

            public void OnMove(T item, int index, int originalIndex)
            {
                var list = _list.Check();
                list.RemoveAt(originalIndex);
                list.Insert(index, item);
            }

            public void OnReplace(T item, T changedItem, int index) => _list.Check()[index] = item;
        }

        private sealed class ToObservableCollection<T> : IListChangeEnumerator<T>
        {
            [NotNull]
            private static readonly ThreadLocal<ToObservableCollection<T>> Factory =
                new ThreadLocal<ToObservableCollection<T>>(() => new ToObservableCollection<T>());

            private ObservableCollection<T> _collection;

            [NotNull]
            public static ToObservableCollection<T> GetInstance([NotNull] ObservableCollection<T> list)
            {
                var result = Factory.Value.NotNull();
                result.SetList(list);
                return result;
            }
            private void SetList(ObservableCollection<T> list)
            {
                _collection = list;
            }

            public void OnStateChanged(IReadOnlyList<T> state)
            {
                throw new System.NotImplementedException();
            }

            public void OnClear() => _collection.Check().Clear();

            public void OnAdd(T item, int index) => _collection.Check().Insert(index, item);

            public void OnRemove(T item, int index) => _collection.Check().RemoveAt(index);

            public void OnMove(T item, int index, int originalIndex) => _collection.Check().Move(originalIndex, index);

            public void OnReplace(T item, T changedItem, int index) => _collection.Check()[index] = item;
        }

        private sealed class ToBindableProxy<T> : IListChangeEnumerator<T>
        {
            [NotNull]
            private static readonly ThreadLocal<ToBindableProxy<T>> Factory =
                new ThreadLocal<ToBindableProxy<T>>(() => new ToBindableProxy<T>());

            private BindableProxy<T> _proxy;
            private bool _reseted;

            [NotNull]
            public static ToBindableProxy<T> GetInstance([NotNull] BindableProxy<T> proxy)
            {
                var result = Factory.Value.NotNull();
                result.SetList(proxy);
                return result;
            }
            private void SetList(BindableProxy<T> proxy)
            {
                _proxy = proxy;
                _reseted = false;
            }

            public void OnStateChanged(IReadOnlyList<T> state)
            {
                _proxy.Check().UnderlyingList = state;
                _reseted = true;
            }

            public void OnClear()
            {
                _reseted = true;
            }

            public void OnAdd(T item, int index)
            {
                if (_reseted) return;
                _proxy.Check().Events.OnAdd(item, index);
            }

            public void OnRemove(T item, int index)
            {
                if (_reseted) return;
                _proxy.Check().Events.OnRemove(item, index);
            }

            public void OnMove(T item, int index, int originalIndex)
            {
                if (_reseted) return;
                _proxy.Check().Events.OnMove(item, index, originalIndex);
            }

            public void OnReplace(T item, T changedItem, int index)
            {
                if (_reseted) return;
                _proxy.Check().Events.OnReplace(item, changedItem, index);
            }
        }

        public static void ApplyTo<T>([NotNull] this IListChange<T> change, [NotNull] IList<T> list)
        {
            var applier = ToList<T>.GetInstance(list);
            change.Enumerate(applier);
        }

        public static void ApplyTo<T>([NotNull] this IListChange<T> change, [NotNull] ObservableCollection<T> list)
        {
            var applier = ToObservableCollection<T>.GetInstance(list);
            change.Enumerate(applier);
        }

        public static void ApplyTo<T>(
            [NotNull] this IListChange<T> change, 
            [NotNull] BindableProxy<T> proxy)
        {
            var applier = ToBindableProxy<T>.GetInstance(proxy);
            change.Enumerate(applier);
        }
    }
}