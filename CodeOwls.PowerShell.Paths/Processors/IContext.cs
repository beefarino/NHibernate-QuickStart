using System.Collections.Generic;
using System.Management.Automation;
using System.Security.AccessControl;
using CodeOwls.PowerShell.Paths.Processors;
using CodeOwls.PowerShell.Provider.PathNodes;

namespace CodeOwls.PowerShell.Provider.PathNodeProcessors
{
    public interface IContext
    {
        string GetResourceString(string baseName, string resourceId);
        void ThrowTerminatingError(ErrorRecord errorRecord);
        bool ShouldProcess(string target);
        bool ShouldProcess(string target, string action);
        bool ShouldProcess(string verboseDescription, string verboseWarning, string caption);
        bool ShouldProcess(string verboseDescription, string verboseWarning, string caption, out ShouldProcessReason shouldProcessReason);
        bool ShouldContinue(string query, string caption);
        bool ShouldContinue(string query, string caption, ref bool yesToAll, ref bool noToAll);
        bool TransactionAvailable();
        void WriteVerbose(string text);
        void WriteWarning(string text);
        void WriteProgress(ProgressRecord progressRecord);
        void WriteDebug(string text);
        void WriteItemObject(object item, string path, bool isContainer);
        void WritePropertyObject(object propertyValue, string path);
        void WriteSecurityDescriptorObject(ObjectSecurity securityDescriptor, string path);
        void WriteError(ErrorRecord errorRecord);
        bool Stopping { get; }
        SessionState SessionState { get; }
        ProviderIntrinsics InvokeProvider { get; }
        CommandInvocationIntrinsics InvokeCommand { get; }
        PSCredential Credential { get; }
        bool Force { get; }
        string Filter { get; }
        IEnumerable<string> Include { get; }
        IEnumerable<string> Exclude { get; }
        object DynamicParameters { get; }
        IPathNodeProcessor PathProcessor { get; }
    }
}