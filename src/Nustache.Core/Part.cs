using System;
using System.Reflection;
using System.Runtime.ExceptionServices;

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
                Type partType = GetType();
                Type visitorType = visitor.GetType();
                MethodInfo visitMethod = visitorType.GetMethod("Visit",
                    bindingAttr: BindingFlags.Instance | BindingFlags.Public,
                    binder: null,
                    types: new Type[] { partType },
                    modifiers: null
                );
                if (visitMethod == null)
                    throw new MissingMemberException($"Visitor of type '{visitorType.FullName}' does not have a Visit method taking a single parameter of type '{partType.FullName}'.");

                visitMethod.Invoke(visitor, new object[] { this });
            }
            catch (TargetInvocationException invokeFailed)
            {
                ExceptionDispatchInfo.Capture(invokeFailed.InnerException).Throw();
            }
        }
    }
}