using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Provider;
using System.Text;
using PostSharp.Aspects;
using PostSharp.Extensibility;

namespace CodeOwls.PowerShell.Provider.Attributes
{
    [Serializable]
    [MulticastAttributeUsage(
        MulticastTargets.Method,
        TargetMemberAttributes = MulticastAttributes.Protected,
        Inheritance = MulticastInheritance.Multicast)]
    [AttributeUsage( AttributeTargets.Class | AttributeTargets.Method)]
    public class LogProviderToSession : OnMethodBoundaryAspect
    {
        public override bool CompileTimeValidate(System.Reflection.MethodBase method)
        {
            return (null != method.DeclaringType.GetProperty("SessionState"));
        }

        public override void OnEntry(MethodExecutionArgs args)
        {
            var cmdlet = args.Instance as CmdletProvider;
            if( null == cmdlet )
            {
                return;
            }

            string parameters = "";
            if( null != args.Arguments && args.Arguments.Any() )
            {
                parameters = String.Join("; ", args.Arguments.ToList().ConvertAll(a => (a ?? "null").ToString()).ToArray());
            }
            cmdlet.WriteDebug( 
                String.Format(
                    "{0} >> Entering {1} ( {2} )", 
                    args.Instance.GetType().FullName, 
                    args.Method.Name,
                    parameters));
        }

        public override void OnExit(MethodExecutionArgs args)
        {            
            var cmdlet = args.Instance as CmdletProvider;
            if (null == cmdlet)
            {
                return;
            }

            cmdlet.WriteDebug(String.Format("{0} << Exiting {1}", args.Instance.GetType().FullName, args.Method.Name));

        }

        public override void OnException(MethodExecutionArgs args)
        {
            var cmdlet = args.Instance as CmdletProvider;
            if (null == cmdlet)
            {
                return;
            }

            cmdlet.WriteDebug(
                String.Format(
                    "{0} !! Exception in {1}: {2}", 
                    args.Instance.GetType().FullName, 
                    args.Method.Name, 
                    args.Exception ));
        }
    }
}
