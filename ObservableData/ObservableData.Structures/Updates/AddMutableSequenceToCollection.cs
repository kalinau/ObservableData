using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using ObservableData.Querying.Core;

namespace ObservableData.Structures.Updates
{
    public sealed class AddMutableSequenceToCollection<T> : EnumerableUpdate<SetOperation<T>>
    {
        [NotNull] private readonly IEnumerable<T> _items;
        [CanBeNull] private IEnumerable<T> _locked;

        public AddMutableSequenceToCollection([NotNull] IEnumerable<T> items)
        {
            _items = items;
        }

        public override void Lock()
        {
            if (_locked != null) return;
            base.CheckAccess();

            _locked = _items.ToList();
        }

        public override IEnumerable<SetOperation<T>> Operations()
        {
            if (_locked != null)
            {
                return Enumerate(_locked);
            }
            base.CheckAccess();
            return Enumerate(_items);
        }

        [NotNull]
        private static IEnumerable<SetOperation<T>> Enumerate([NotNull] IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                yield return SetOperation<T>.OnAdd(item);
            }
        }
    }
}
