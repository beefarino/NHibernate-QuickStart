using System;
using System.Collections;
using NHibernate.Linq;
using NHibernate.Metadata;
using Enumerable = System.Linq.Enumerable;

namespace NHibernate.DriveProvider
{
    class QueryHelper<T> : IQueryHelper where T : class
    {
        public IEnumerable GetAll(ISession session)
        {
            return Enumerable.ToArray<T>(session.Query<T>());
        }

        public object GetById( IClassMetadata metadata, ISession session, object id )
        {
            id = ConvertId(metadata, id);
            return session.Get<T>(id);
        }

        private static object ConvertId(IClassMetadata metadata, object id)
        {
            var type = metadata.IdentifierType;
            id = Convert.ChangeType(id, type.ReturnedClass);
            return id;
        }

        public void DeleteById(IClassMetadata metadata, ISession session, object id)
        {
            object target = GetById(metadata, session, id);
            session.Delete(target);
        }
    }
}