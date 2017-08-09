using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using ObservableData.Querying.Core;

namespace ObservableData.Structures.Updates
{
    public sealed class ResetWithMutableSequenceToList<T> : EnumerableUpdate<ListOperation<T>>
    {
        [NotNull] private readonly IEnumerable<T> _items;
        [CanBeNull] private IEnumerable<T> _locked;

        public ResetWithMutableSequenceToList([NotNull] IEnumerable<T> items)
        {
            _items = items;
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
                return Enumerate(_locked);
            }
            base.CheckAccess();
            return Enumerate(_items);
        }

        [NotNull]
        private static IEnumerable<ListOperation<T>> Enumerate([NotNull] IEnumerable<T> items)
        {
            var index = 0;
            yield return ListOperation<T>.OnClear();
            foreach (var item in items)
            {
                yield return ListOperation<T>.OnAdd(item, index++);
            }
        }
    }
}