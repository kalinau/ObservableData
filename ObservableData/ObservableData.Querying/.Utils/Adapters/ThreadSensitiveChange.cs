using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace ObservableData.Querying.Utils.Adapters
{
    public abstract class ThreadSensitiveChange<T> : IBatch<T>
    {
        [CanBeNull] private IEnumerable<T> _locked;
        private ThreadId _threadId;

        protected ThreadSensitiveChange()
        {
            _threadId = ThreadId.FromCurrent();
        }

        public IEnumerable<T> GetPeaces()
        {
            if (_locked != null) return _locked;
            _threadId.CheckIsCurrent();
            return this.Enumerate();
        }

        public void MakeImmutable()
        {
            if (_locked != null) return;
            _threadId.CheckIsCurrent();
            _locked = this.Enumerate().ToArray();
        }

        [NotNull]
        protected abstract IEnumerable<T> Enumerate();
    }
}