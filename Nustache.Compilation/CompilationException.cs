using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nustache.Compilation
{
    [Serializable]
    public class CompilationException : Exception
    {
        public CompilationException(string message, string context, int row, int col)
            : base(message + /*" at row: " + row + ", col: " + col + */"\nOn object: " + context) { }

        protected CompilationException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
