using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using ObservableData.Querying.Utils;

namespace ObservableData.Querying.Select
{
    internal static partial class SelectImmutable
    {
        private struct ItemCounter<T>
        {
            private readonly uint _count;
            private readonly T _item;

            public ItemCounter(T item, uint count)
            {
                _count = count;
                _item = item;
            }

            public T Item => _item;

            public uint Count => _count;
        }

        private sealed class Map<TKey, T> 
        {
            [NotNull] private Dictionary<TKey, ItemCounter<T>> _state;

            public Map() : this(new Dictionary<TKey, ItemCounter<T>>())
            {
            }

            private Map([NotNull] Dictionary<TKey, ItemCounter<T>> dictionary)
            {
                _state = dictionary;
            }

            public bool IsEmpty => _state.Count == 0;

            public T this[TKey key] => _state[key].Item;

            public void Clear()
            {
                _state.Clear();
            }

            public Map<TKey, T> CloneAndClear()
            {
                var clone = new Map<TKey, T>(_state);
                _state = new Dictionary<TKey, ItemCounter<T>>(_state.Count);
                return clone;
            }

            public void Add(TKey key, T value)
            {
                _state.Add(key, new ItemCounter<T>(value, 1));
            }

            public bool TryGet(TKey key, out T result)
            {
                if (_state.TryGetValue(key, out var existing))
                {
                    result = existing.Item;
                    return true;
                }
                result = default(T);
                return false;
            }

            public bool TryIncreaseCount(TKey key)
            {
                if (_state.TryGetValue(key, out var existing))
                {
                    _state[key] = new ItemCounter<T>(existing.Item, existing.Count + 1);
                    return true;
                }
                return false;
            }

            public void DecreaseCount(TKey key)
            {
                if (!_state.TryGetValue(key, out var existing))
                {
                    throw new ArgumentOutOfRangeException();
                }
                //currentValue = existing.Item;
                if (existing.Count > 1)
                {
                    _state[key] = new ItemCounter<T>(existing.Item, existing.Count - 1);
                }
                else
                {
                    _state.Remove(key);
                }
                //return existing.Count;
            }
        }

        private static void OnAdd<TKey, T>(
            [NotNull] this Map<TKey, T> state,
            TKey item,
            [NotNull] Func<TKey, T> selector,
            [CanBeNull] Dictionary<TKey, T> removedOnChange)
        {
            if (state.TryIncreaseCount(item)) return;

            var result = default(T);
            if (removedOnChange?.TryGetValue(item, out result) != true)
            {
                result = selector(item);
            }
            state.Add(item, result);
        }

        private static void OnRemove<TKey, T>(
            [NotNull] this Map<TKey, T> state,
            TKey item,
            [CanBeNull] ref Dictionary<TKey, T> removedOnChange)
        {
            //if (state.DecreaseCount(item, out var removed) <= 1)
            //{
            //    if (removedOnChange == null)
            //    {
            //        removedOnChange = new Dictionary<TKey, T>();
            //    }
            //    removedOnChange[item] = removed;
            //}
        }

        private static T Get<TKey, T>(
            [NotNull] this Map<TKey, T> state,
            TKey key,
            [CanBeNull] IReadOnlyDictionary<TKey, T> removedOnChange)
        {
            if (!state.TryGet(key, out var item))
            {
                if (removedOnChange != null)
                {
                    return removedOnChange[key];
                }
                throw new ArgumentOutOfRangeException();
            }
            return item;
        }
    }
}
