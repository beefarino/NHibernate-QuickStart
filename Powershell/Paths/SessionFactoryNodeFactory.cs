using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeOwls.PowerShell.Provider.PathNodes;
using NHibernate.DriveProvider.Items;
using NHibernate.Metadata;

namespace NHibernate.DriveProvider.Paths
{
    class SessionFactoryNodeFactory : NodeFactoryBase
    {
        private readonly ISession _session;
        private readonly ISessionFactory _sessionFactory;
        private readonly string _name;

        public SessionFactoryNodeFactory(string name, ISessionFactory sessionFactory, ISession session )
        {
            _name = name;
            _session = session;
            _sessionFactory = sessionFactory;
        }

        public override IEnumerable<INodeFactory> GetNodeChildren()
        {
            var metadata = _sessionFactory.GetAllClassMetadata();
            foreach( IClassMetadata item in metadata.Values )
            {
                yield return new ClassMetadataNodeFactory( _session, item );
            }
        }

        public override IPathNode GetNodeValue()
        {
            return new PathNode( new SessionFactory(_sessionFactory), Name, true );
        }

        public override string Name
        {
            get { return _name; }
        }
    }
}
