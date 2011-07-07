using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using NHibernate.Engine;
using NHibernate.Metadata;
using NHibernate.Stat;

namespace NHibernate.DriveProvider.Items
{
    public class SessionFactory
    {
        private readonly ISessionFactory _sessionFactory;

        public SessionFactory(ISessionFactory sessionFactory)
        {
            _sessionFactory = sessionFactory;
        }

        public ISession OpenSession(IDbConnection conn)
        {
            return _sessionFactory.OpenSession(conn);
        }

        public ISession OpenSession(IInterceptor sessionLocalInterceptor)
        {
            return _sessionFactory.OpenSession(sessionLocalInterceptor);
        }

        public ISession OpenSession(IDbConnection conn, IInterceptor sessionLocalInterceptor)
        {
            return _sessionFactory.OpenSession(conn, sessionLocalInterceptor);
        }

        public ISession OpenSession()
        {
            return _sessionFactory.OpenSession();
        }

        public IClassMetadata GetClassMetadata(System.Type persistentClass)
        {
            return _sessionFactory.GetClassMetadata(persistentClass);
        }

        public IClassMetadata GetClassMetadata(string entityName)
        {
            return _sessionFactory.GetClassMetadata(entityName);
        }

        public ICollectionMetadata GetCollectionMetadata(string roleName)
        {
            return _sessionFactory.GetCollectionMetadata(roleName);
        }

        public IDictionary<string,IClassMetadata> GetAllClassMetadata()
        {
            return _sessionFactory.GetAllClassMetadata();
        }

        public IDictionary<string, ICollectionMetadata> GetAllCollectionMetadata()
        {
            return _sessionFactory.GetAllCollectionMetadata();
        }

        public void Close()
        {
            _sessionFactory.Close();
        }

        public void Evict(System.Type persistentClass)
        {
            _sessionFactory.Evict(persistentClass);
        }

        public void Evict(System.Type persistentClass, object id)
        {
            _sessionFactory.Evict(persistentClass, id);
        }

        public void EvictEntity(string entityName)
        {
            _sessionFactory.EvictEntity(entityName);
        }

        public void EvictEntity(string entityName, object id)
        {
            _sessionFactory.EvictEntity(entityName, id);
        }

        public void EvictCollection(string roleName)
        {
            _sessionFactory.EvictCollection(roleName);
        }

        public void EvictCollection(string roleName, object id)
        {
            _sessionFactory.EvictCollection(roleName, id);
        }

        public void EvictQueries()
        {
            _sessionFactory.EvictQueries();
        }

        public void EvictQueries(string cacheRegion)
        {
            _sessionFactory.EvictQueries(cacheRegion);
        }

        public IStatelessSession OpenStatelessSession()
        {
            return _sessionFactory.OpenStatelessSession();
        }

        public IStatelessSession OpenStatelessSession(IDbConnection connection)
        {
            return _sessionFactory.OpenStatelessSession(connection);
        }

        public FilterDefinition GetFilterDefinition(string filterName)
        {
            return _sessionFactory.GetFilterDefinition(filterName);
        }

        public ISession GetCurrentSession()
        {
            return _sessionFactory.GetCurrentSession();
        }

        public IStatistics Statistics
        {
            get { return _sessionFactory.Statistics; }
        }

        public bool IsClosed
        {
            get { return _sessionFactory.IsClosed; }
        }

        public ICollection<string> DefinedFilterNames
        {
            get { return _sessionFactory.DefinedFilterNames; }
        }
    }
}
