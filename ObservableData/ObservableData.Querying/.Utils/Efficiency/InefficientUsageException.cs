using System;
using JetBrains.Annotations;

namespace ObservableData.Querying.Utils.Efficiency
{
    [PublicAPI]
    public class InefficientUsageException : Exception
    {
        public InefficientUsageException()
        {
        }

        public InefficientUsageException(string message) : base(message)
        {
        }

        public InefficientUsageException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
