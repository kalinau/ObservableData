using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using ObservableData.Structures;
using ObservableData.Structures.Utils;

namespace ObservableData.Querying.Utils.Adapters
{
    public abstract class ChangeWithLock<T> : IChange<T>
    {
        [CanBeNull] private IEnumerable<T> _locked;
        private ThreadId _threadId;

        protected ChangeWithLock()
        {
            _threadId = ThreadId.FromCurrent();
        }

        public void Lock()
        {
            if (_locked != null) return;
            _threadId.CheckIsCurrent();
            _locked = this.Enumerate().ToArray();
        }

        public IEnumerable<T> Operations()
        {
            if (_locked != null) return _locked;
            _threadId.CheckIsCurrent();
            return this.Enumerate();
        }

        [NotNull]
        protected abstract IEnumerable<T> Enumerate();
    }
}