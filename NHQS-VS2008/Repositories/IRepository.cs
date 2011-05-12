using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using NHibernate.QuickStart.Components;

namespace NHibernate.QuickStart.Repositories
{
    public interface IRepository<T> : IRepository<T, int>
    {
    }

    public interface IRepository<T, TId> : IDisposable
    {
        T Add(T target);
        T Update(T target);
        T Get(TId id);
        void Delete(T target);
        List<T> Find(Expression<Func<T, bool>> expression);
        ISession Session { get; set; }
    }
}
