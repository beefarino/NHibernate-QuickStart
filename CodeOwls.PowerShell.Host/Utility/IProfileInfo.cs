using System.Collections.Generic;
using System.Management.Automation;

namespace CodeOwls.PowerShell.Host.Utility
{
    public interface IProfileInfo
    {
        string AllUsersAllHosts { get; }
        string AllUsersPowerShellHost { get; }
        string CurrentUserAllHosts { get; }
        string CurrentUserPowerShellHost { get; }
        string AllUsersCurrentHost { get; }
        string CurrentUserCurrentHost { get; }
        IEnumerable<string> InRunOrder { get; }
        PSObject GetProfilePSObject();
    }
}