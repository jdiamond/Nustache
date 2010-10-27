using System;
using System.Runtime.Serialization;

namespace Nustache.Core
{
    [Serializable]
    public class NustacheException : Exception
    {
        public NustacheException()
        {
        }

        public NustacheException(string message)
            : base(message)
        {
        }

        public NustacheException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected NustacheException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}