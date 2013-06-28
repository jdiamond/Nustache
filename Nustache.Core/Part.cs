using System;
namespace Nustache.Core
{
    public abstract class Part
    {
        public abstract void Render(RenderContext context);

        public abstract string Source();

        public void Accept(PartVisitor visitor)
        {
            try
            {
                visitor.GetType().GetMethod("Visit", new Type[] { this.GetType() })
                    .Invoke(visitor, new object[] { this });
            }
            catch (System.Reflection.TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }
    }
}