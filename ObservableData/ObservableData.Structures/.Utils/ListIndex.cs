using System;
using System.Runtime.CompilerServices;

namespace ObservableData.Structures.Utils
{
    public static class ListIndex
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Check(int index, int count)
        {
            if (index < count && index >= 0)
            {
                return;
            }
            throw new ArgumentOutOfRangeException();
        }
    }
}
