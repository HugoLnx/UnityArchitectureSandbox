using System;
using System.Runtime.Serialization;

namespace LnxArch
{
    [Serializable]
    internal class LnxCircularDependencyException : Exception
    {
        public LnxCircularDependencyException()
        {
        }

        public LnxCircularDependencyException(string message) : base(message)
        {
        }

        public LnxCircularDependencyException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected LnxCircularDependencyException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}