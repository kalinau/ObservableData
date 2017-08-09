using System.Collections.Generic;
using JetBrains.Annotations;

namespace ObservableData.Structures.Utils
{
    public struct ItemOrEnumerable<T>
    {
        [CanBeNull]
        private readonly IEnumerable<T> _items;
        private readonly T _item;

        public ItemOrEnumerable(IEnumerable<T> items)
        {
            _items = items;
            _item = default(T);
        }

        public ItemOrEnumerable(T item)
        {
            _items = null;
            _item = item;
        }

        [CanBeNull]
        public IEnumerable<T> Items => _items;

        public T Item => _item;
    }
}
