using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation.Provider;
using System.Text;
using CodeOwls.PowerShell.Paths.Processors;
using CodeOwls.PowerShell.Provider;
using CodeOwls.PowerShell.Provider.PathNodes;

namespace NHQSProvider
{
    [CmdletProvider( "ALO", ProviderCapabilities.None)]
    public class Class1 : Provider, IPathNodeProcessor
    {
        protected override IPathNodeProcessor PathNodeProcessor
        {
            get { return this; }
        }

        public INodeFactory ResolvePath(string path)
        {
            throw new NotImplementedException();
        }

        protected override System.Management.Automation.PSDriveInfo NewDrive(System.Management.Automation.PSDriveInfo drive)
        {
            return base.NewDrive(drive);
        }
    }
}
