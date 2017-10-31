namespace Nustache.Core
{
    public interface ValueGetterVisitor
    {
        void Visit(MethodInfoValueGetter getter);
        void Visit(PropertyInfoValueGetter getter);
        void Visit(FieldInfoValueGetter getter);
        void Visit(GenericDictionaryValueGetter getter);

        void NoMatch(ValueGetter valueGetter);
    }
}
