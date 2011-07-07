using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeOwls.PowerShell.Provider.PathNodes;

namespace NHibernate.DriveProvider.Items
{
    class DomainPathNode : PathNode
    {
        public DomainPathNode(object item, string name, bool isContainer) : base(item, name, isContainer)
        {
        }
    }

    
}
