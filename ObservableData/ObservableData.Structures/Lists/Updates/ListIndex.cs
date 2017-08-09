using System;

namespace ObservableData.Structures.Lists.Updates
{
    public static class ListIndex
    {
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
