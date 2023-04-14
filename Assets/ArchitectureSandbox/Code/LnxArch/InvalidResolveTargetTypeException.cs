using System;

namespace LnxArch
{
    public class InvalidResolveTargetTypeException : Exception
    {
        public InvalidResolveTargetTypeException(string message) : base(message)
        {
        }

        public static InvalidResolveTargetTypeException BuildUnsupportedCollectionType()
        {
            return new("LnxEntity can resolve List and Array, but no other collection types are supported");
        }
    }
}