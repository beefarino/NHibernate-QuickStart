using CodeOwls.PowerShell.Provider.PathNodeProcessors;
using CodeOwls.PowerShell.Provider.PathNodes;
using NHibernate.DriveProvider.Items;
using NHibernate.DriveProvider.Utility;
using NHibernate.Metadata;

namespace NHibernate.DriveProvider.Paths
{
    class EntityObjectNodeFactory : NodeFactoryBase, IRemoveItem, ISetItem
    {
        private string _name;
        private readonly object _item;
        private readonly ISession _session;
        private readonly IClassMetadata _metadata;

        public EntityObjectNodeFactory(object item, ISession session, IClassMetadata metadata)
        {
            _item = item;
            _session = session;
            _metadata = metadata;
        }

        public override IPathNode GetNodeValue()
        {
            return new DomainPathNode( _item, Name, false );
        }

        public override string Name
        {
            get
            {
                if( null != _name )
                {
                    return _name;
                }

                _name = _metadata.GetIdentifier(_item, EntityMode.Poco).ToString();
                return _name;
            }
        }

        public object RemoveItemParameters
        {
            get { return null; }
        }

        public void RemoveItem(IContext context, string path, bool recurse)
        {
            var helper = _metadata.ToQueryHelper();
            helper.DeleteById(_metadata, _session, path);
        }

        public object SetItemParameters
        {
            get { return _metadata.ToDynamicParameters(); }
        }

        public IPathNode SetItem(IContext context, string path, object value)
        {
            var prototype = _metadata.ConvertDynamicParametersToPrototype(context, value);

            _session.Merge(prototype);
            
            return new DomainPathNode(prototype, path, false);
        }
    }
}