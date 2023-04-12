using System;
using System.Runtime.Serialization;

namespace LnxArch
{
    [Serializable]
    internal class InvalidReadingOverwrite : Exception
    {
        public InvalidReadingOverwrite()
        {
        }

        public InvalidReadingOverwrite(string message) : base(message)
        {
        }

        public InvalidReadingOverwrite(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvalidReadingOverwrite(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}