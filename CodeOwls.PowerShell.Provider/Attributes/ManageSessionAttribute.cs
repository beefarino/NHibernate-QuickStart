using System;
using PostSharp.Aspects;
using PostSharp.Extensibility;

namespace CodeOwls.PowerShell.Provider.Attributes
{
    [Serializable]
    [MulticastAttributeUsage( 
        MulticastTargets.Method, 
        TargetMemberAttributes = MulticastAttributes.Protected, 
        Inheritance = MulticastInheritance.Multicast)]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class ManageSessionAttribute : MethodInterceptionAspect
    {
        public override bool CompileTimeValidate(System.Reflection.MethodBase method)
        {            
            if( ! typeof(IProvideNewSession).IsAssignableFrom(method.DeclaringType))
            {
                Message.Write( 
                    SeverityType.Error, 
                    "ManageSessionAttribute001", 
                    "Cannot apply [ManageSession] to method {0} because it is not a member of a type derived from IProvideNewSession.", 
                    method
                );

                return false;
            }

            return true;
        }

        public override void OnInvoke(MethodInterceptionArgs args)
        {
            if( args.Method.IsConstructor || args.Method.Name == "NewSession")
            {
                args.Proceed();
            }

            var c = args.Instance as IProvideNewSession;

            using( c.NewSession() )
            {
                args.Proceed();
            }
        }
    }
}
