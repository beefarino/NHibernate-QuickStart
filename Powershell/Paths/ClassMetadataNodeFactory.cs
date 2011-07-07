using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Text;
using CodeOwls.PowerShell.Provider.PathNodeProcessors;
using CodeOwls.PowerShell.Provider.PathNodes;
using NHibernate.DriveProvider.Items;
using NHibernate.DriveProvider.Utility;
using NHibernate.Metadata;

namespace NHibernate.DriveProvider.Paths
{
    class ClassMetadataNodeFactory : NodeFactoryBase, INewItem
    {
        private readonly ISession _session;
        private readonly IClassMetadata _metadata;

        public ClassMetadataNodeFactory( ISession session, IClassMetadata metadata )
        {            
            _session = session;
            _metadata = metadata;
        }

        public override IEnumerable<INodeFactory> GetNodeChildren()
        {
            IQueryHelper helper = _metadata.ToQueryHelper();
            var e = helper.GetAll(_session);
            foreach( var i in e )
            {
                yield return new EntityObjectNodeFactory(i, _session, _metadata);
            }
        }


        public override INodeFactory Resolve(string nodeName)
        {
            IQueryHelper helper = _metadata.ToQueryHelper();
            var value = helper.GetById(_metadata, _session, nodeName);
            return new EntityObjectNodeFactory(value, _session, _metadata);
        }

        public override IPathNode GetNodeValue()
        {
            return new PathNode( new ClassMetadataNodeFactory( _session,_metadata), Name, true);
        }

        public override string Name
        {
            get { return _metadata.GetMappedClass(EntityMode.Poco).Name; }
        }

        public IEnumerable<string> NewItemTypeNames
        {
            get { return null; }
        }

        public object NewItemParameters
        {
            get 
            {
                return _metadata.ToDynamicParameters();
            }
        }
        
        public IPathNode NewItem(IContext context, string path, string itemTypeName, object newItemValue)
        {
            System.Type type;
            var prototype = _metadata.ConvertDynamicParametersToPrototype(context, newItemValue, out type);

            _session.Save(prototype);
            var id = type.GetProperty(_metadata.IdentifierPropertyName).GetValue(prototype, null);
            
            return new DomainPathNode(prototype, id.ToString(), false);
        }
    }
}
