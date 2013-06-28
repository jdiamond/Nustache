using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nustache.Core
{
    internal interface ValueGetterVisitor
    {
        void Visit(MethodInfoValueGetter getter);
        void Visit(PropertyInfoValueGetter getter);
        void Visit(FieldInfoValueGetter getter);
        void Visit(GenericDictionaryValueGetter getter);

        void NoMatch(ValueGetter valueGetter);
    }
}
