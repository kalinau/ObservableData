using System.Diagnostics;
using JetBrains.Annotations;

namespace ObservableData.Querying.Utils.Efficiency
{
    //TODO: verify
    [PublicAPI]
    public static class Inneficient
    {
        public static void Assert(bool condition)
        {
            if (!condition && Debugger.IsAttached)
            {
                throw new InefficientUsageException();
            }
        }

        public static void Assert(bool condition, string message)
        {
            if (!condition && Debugger.IsAttached)
            {
                throw new InefficientUsageException(message);
            }
        }
    }
}