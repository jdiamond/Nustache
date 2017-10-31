
namespace Nustache.Core
{
    public interface PartVisitor
    {
        void Visit(Section section);
        void Visit(Block block);
        void Visit(LiteralText literal);
        void Visit(EndSection endSections);
        void Visit(InvertedBlock invertedBlock);
        void Visit(TemplateInclude include);
        void Visit(VariableReference variable);
    }
}
