using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.QuickStart.Components;

namespace NHibernate.QuickStart.Repositories
{
    public static class NHibernateSessionExtensions
    {
        public static BaseRepository<T> CreateRepository<T>(this ISession session)
        {
            return new BaseRepository<T> { Session = session };
        }
    }
}