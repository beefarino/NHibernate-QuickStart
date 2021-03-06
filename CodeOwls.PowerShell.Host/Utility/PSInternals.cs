using System.Management.Automation;
using System.Reflection;

namespace CodeOwls.PowerShell.Host.Utility
{
    internal static class PSInternals
    {
        public static PSTraceSource GetTracer(string name, string description)
        {
            var mi = typeof (PSTraceSource).GetMethod("GetTracer", BindingFlags.Static | BindingFlags.NonPublic);
            if (null == mi)
            {
                return null;
            }

            var result = mi.Invoke(null, new[] {name, description});
            return result as PSTraceSource;
        }
    }
}