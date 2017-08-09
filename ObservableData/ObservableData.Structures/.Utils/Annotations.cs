using System;
using System.Diagnostics;
using JetBrains.Annotations;

namespace ObservableData.Structures.Utils
{
    internal static class Annotations
    {
        [NotNull]
        [ContractAnnotation("value:null => halt;value:notnull => notnull")]
        public static T NotNull<T>([NoEnumeration][CanBeNull] this T value) where T : class
        {
            Debug.Assert(value != null);
            if (value == null)
            {
                throw new Exception("Violated contract, expected not null value.");
            }
            return value;
        }
    }
}
