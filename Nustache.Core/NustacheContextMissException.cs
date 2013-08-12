using System;

namespace Nustache.Core
{
    [Serializable]
    public class NustacheContextMissException : NustacheException
    {
        public NustacheContextMissException(string message)
            : base(message)
        {
        }
    }
}