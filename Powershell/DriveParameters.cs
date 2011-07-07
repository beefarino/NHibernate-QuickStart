using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management.Automation;

namespace NHQS.DriveProvider
{
    public class DriveParameters
    {
        [Parameter(Mandatory = true)]
        public string DomainAssembly { get; set; }

        [Parameter(Mandatory = false)]
        public string DataAccessAssembly { get; set; }

        [Parameter(Mandatory = false, ParameterSetName = "ConfigFile")]
        [Alias( "AppConfig", "Config" )]
        public string ConfigurationFile { get; set; }

        [Parameter(Mandatory = false, ParameterSetName = "ConnectionString")]
        public Hashtable ConnectionString { get; set; }
    }
}
