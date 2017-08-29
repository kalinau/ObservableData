using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace ObservableData.Querying.Utils.Adapters
{
    public class CurrentStateChange<T> : IBatch<GeneralChange<T>>, IBatch<IndexedChange<T>>
    {
        [NotNull] private IReadOnlyCollection<T> _items;
        private ThreadId? _threadId;

        public CurrentStateChange([NotNull] IReadOnlyCollection<T> items)
        {
            _items = items;
            _threadId = ThreadId.FromCurrent();
        }

        public void MakeImmutable()
        {
            if (_threadId != null)
            {
                _threadId?.CheckIsCurrent();
                _items = _items.ToList();
                _threadId = null;
            }
        }

        IEnumerable<GeneralChange<T>> IBatch<GeneralChange<T>>.GetPeaces()
        {
            _threadId?.CheckIsCurrent();
            foreach (var item in _items)
            {
                yield return GeneralChange<T>.OnAdd(item);
            }
        }


        IEnumerable<IndexedChange<T>> IBatch<IndexedChange<T>>.GetPeaces()
        {
            _threadId?.CheckIsCurrent();
            int i = 0;
            foreach (var item in _items)
            {
                yield return IndexedChange<T>.OnAdd(item, i++);
            }
        }
    }
}