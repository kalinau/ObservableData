using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using ObservableData.Querying.Core;

namespace ObservableData.Structures.Updates
{
    public sealed class AddMutableSequenceToList<T> : EnumerableUpdate<ListOperation<T>>
    {
        [NotNull] private readonly IEnumerable<T> _items;
        [CanBeNull] private IEnumerable<T> _locked;
        private readonly int _index;

        public AddMutableSequenceToList([NotNull] IEnumerable<T> items, int index)
        {
            _items = items;
            _index = index;
        }

        public override void Lock()
        {
            if (_locked != null) return;
            base.CheckAccess();

            _locked = _items.ToList();
        }

        public override IEnumerable<ListOperation<T>> Operations()
        {
            if (_locked != null)
            {
                return Enumerate(_locked, _index);
            }
            base.CheckAccess();
            return Enumerate(_items, _index);
        }

        [NotNull]
        private static IEnumerable<ListOperation<T>> Enumerate([NotNull] IEnumerable<T> items, int index)
        {
            foreach (var item in items)
            {
                yield return ListOperation<T>.OnAdd(item, index++);
            }
        }
    }
}