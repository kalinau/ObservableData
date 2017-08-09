using System;
using JetBrains.Annotations;

namespace ObservableData.Querying.Utils
{
    [PublicAPI]
    public static class WeakReferenceExtensions
    {
        [CanBeNull]
        public static T TryGetTarget<T>([NotNull] this WeakReference<T> reference) where T : class
        {
            T o;
            reference.TryGetTarget(out o);
            return o;
        }
    }
}