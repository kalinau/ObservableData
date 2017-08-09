using System;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace ObservableData.Structures.Utils
{
    public struct ThreadId
    {
        private readonly int? _threadId;

        private ThreadId(int threadId)
        {
            _threadId = threadId;
        }

        public static ThreadId FromCurrent()
        {
            return new ThreadId(Environment.CurrentManagedThreadId);
        }

        [Pure]
        [SuppressMessage("ReSharper", "PureAttributeOnVoidMethod")]
        public void CheckIsCurrent()
        {
            if (_threadId != Environment.CurrentManagedThreadId)
            {
                throw new InvalidOperationException("incorrect thread");
            }
        }
    }
}