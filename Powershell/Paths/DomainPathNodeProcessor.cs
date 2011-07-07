using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using CodeOwls.PowerShell.Paths.Processors;
using CodeOwls.PowerShell.Provider.PathNodes;
using NHQS;
using NHQS.DriveProvider;

namespace NHibernate.DriveProvider.Paths
{
    class DomainPathNodeProcessor : PathNodeProcessorBase
    {
        private readonly ISessionFactory _sessionFactory;
        private readonly DriveParameters _driveParameters;
        private readonly INodeFactory _root;
        private System.Type _domainObjectType;
        private ISession _session;


        public DomainPathNodeProcessor(string rootName, ISessionFactory sessionFactory, ISession session)
        {
            _sessionFactory = sessionFactory;
            _session = session;
            _root = new SessionFactoryNodeFactory(rootName, _sessionFactory, _session);
        }


        protected override INodeFactory Root { get { return _root; } }
    }
}
