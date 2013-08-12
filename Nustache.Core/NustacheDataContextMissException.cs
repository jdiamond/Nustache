using System;

namespace Nustache.Core
{
    [Serializable]
    public class NustacheDataContextMissException : NustacheException
    {
        public NustacheDataContextMissException(string message)
            : base(message)
        {
        }
    }
}