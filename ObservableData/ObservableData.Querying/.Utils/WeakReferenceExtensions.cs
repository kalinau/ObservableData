using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace ObservableData.Querying.Utils
{
    [PublicAPI]
    public static class WeakReferenceExtensions
    {
        [CanBeNull]
        public static T TryGetTarget<T>([NotNull] this WeakReference<T> reference) where T : class
        {
            reference.TryGetTarget(out var o);
            return o;
        }
    }
}