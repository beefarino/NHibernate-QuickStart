using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management.Automation;

namespace NHQS.DriveProvider
{
    public class DriveParameters
    {
        [Parameter(Mandatory = false)]
        public string DomainAssembly { get; set; }

        [Parameter(Mandatory = false)]
        public string DataAccessAssembly { get; set; }
    }
}
