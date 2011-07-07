using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using NHibernate.Metadata;

namespace NHibernate.DriveProvider
{
    interface IQueryHelper
    {
        IEnumerable GetAll(ISession session);
        object GetById(IClassMetadata metadata, ISession session, object id);
        void DeleteById(IClassMetadata metadata, ISession session, object id);
    }
}
