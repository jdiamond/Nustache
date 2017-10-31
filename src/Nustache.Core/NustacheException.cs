using System;

namespace Nustache.Core
{
    [Serializable]
    public class NustacheException : Exception
    {
        public NustacheException(string message)
            : base(message)
        {
        }
    }
}