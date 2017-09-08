using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using ObservableData.Querying.Utils;

namespace ObservableData.Querying.Where
{
    internal sealed class CollectionState<T> : IReadOnlyCollection<T>
    {
        [NotNull] private IReadOnlyCollection<T> _source = EmptyList<T>.Instance;
        [NotNull] private readonly Func<T, bool> _criterion;

        private int? _count;

        public CollectionState([NotNull] Func<T, bool> criterion)
        {
            _criterion = criterion;
        }

        int IReadOnlyCollection<T>.Count
        {
            get
            {
                if (_count == null)
                {
                    _count = _source.Count(_criterion);
                }
                return _count.Value;
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _source.Where(_criterion).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        public void ChangeState([NotNull] IReadOnlyCollection<T> state)
        {
            _source = state;
            _count = 0;
        }

        public void Clear()
        {
            _count = 0;
        }

        public void IncreaseCount()
        {
            if (_count != null)
            {
                _count++;
            }
        }

        public void DecreaseCount()
        {
            if (_count != null)
            {
                _count--;
            }
        }
    }
}