using System.Management.Automation;

namespace CodeOwls.PowerShell.Host.Utility
{
    internal static class PSObjectExtensions
    {
        public static string ToStringValue(this PSObject o)
        {
            if (null == o)
            {
                return null;
            }
            if (null != o.BaseObject)
            {
                return o.BaseObject.ToString();
            }
            return o.ToString();
        }
    }
}