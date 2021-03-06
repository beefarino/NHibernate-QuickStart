using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using CodeOwls.PowerShell.Host.Utility;

namespace CodeOwls.PowerShell.Host.Configuration
{
    public class ShellConfiguration
    {
        public ShellConfiguration()
        {
            UISettings = new UISettings();
            InitialVariables = new List<PSVariable>();
            Cmdlets = new List<CmdletConfigurationEntry>();
        }

        public string ShellName { get; set; }
        public Version ShellVersion { get; set; }
        public List<PSVariable> InitialVariables { get; set; }
        public UISettings UISettings { get; set; }
        public RunspaceConfiguration RunspaceConfiguration { get; set; }
        public IProfileInfo ProfileScripts { get; set; }

        public List<CmdletConfigurationEntry> Cmdlets { get; set; }
    }
}