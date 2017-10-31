using System;

namespace Nustache.Core
{
    [Serializable]
    public class NustacheEmptyStringException : NustacheException
    {
        public NustacheEmptyStringException(string message)
            : base(message)
        {
        }
    }
}