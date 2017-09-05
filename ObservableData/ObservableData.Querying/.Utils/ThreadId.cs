using System;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace ObservableData.Querying.Utils
{
    public struct ThreadId
    {
        private readonly int _id;

        private ThreadId(int id)
        {
            _id = id;
        }

        public int Id => _id;

        public static ThreadId FromCurrent()
        {
            return new ThreadId(Environment.CurrentManagedThreadId);
        }
    }

    public static class ThreadIdExtensions
    {
        [Pure]
        [SuppressMessage("ReSharper", "PureAttributeOnVoidMethod")]
        public static void CheckIsCurrent(this ThreadId thread)
        {
            if (thread.Id != Environment.CurrentManagedThreadId)
            {
                throw new InvalidOperationException("incorrect thread");
            }
        }

        [Pure]
        [SuppressMessage("ReSharper", "PureAttributeOnVoidMethod")]
        public static void CheckIsCurrent(this ThreadId? thread)
        {
            if (thread?.Id != Environment.CurrentManagedThreadId)
            {
                throw new InvalidOperationException("incorrect thread");
            }
        }


        [NotNull]
        [ContractAnnotation("value:null => halt;value:notnull => notnull")]
        public static T Check<T>([NoEnumeration][CanBeNull] this T value, ThreadId? thread) 
            where T : class
        {
            thread.CheckIsCurrent();
            if (value == null)
            {
                throw new InvalidOperationException("incorrect thread");
            }
            return value;
        }

        [NotNull]
        [ContractAnnotation("value:null => halt;value:notnull => notnull")]
        public static T Check<T>([NoEnumeration][CanBeNull] this T value)
            where T : class
        {
            if (value == null)
            {
                throw new InvalidOperationException("incorrect thread");
            }
            return value;
        }
    }
}