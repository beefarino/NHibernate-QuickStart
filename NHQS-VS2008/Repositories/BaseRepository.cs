using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Text;
using System.Linq.Dynamic;
using NHibernate.Linq;
using NHibernate.QuickStart.Components;

namespace NHibernate.QuickStart.Repositories
{
	public class BaseRepository<T> : BaseRepository<T, int>
	{
	}

	public class BaseRepository<T, TId> : IRepository<T, TId>
	{
		#region IRepository<T,TId> Members

		public T Add(T target)
		{
			this.Session.BeginTransaction();

			try
			{
				this.Session.Save(target);
				this.Session.Transaction.Commit();
			}
			catch (Exception x)
			{
				this.Session.Transaction.Rollback();
				throw x;
			}

			return target;
		}

		public T Update(T target)
		{
			this.Session.BeginTransaction();

			try
			{
				this.Session.Update(target);
				this.Session.Transaction.Commit();
			}
			catch (Exception x)
			{
				this.Session.Transaction.Rollback();
				throw x;
			}

			return target;
		}

		public T Get(TId id)
		{
			T ret = default(T);

			this.Session.BeginTransaction();

			try
			{
				ret = this.Session.Get<T>(id);
				this.Session.Transaction.Commit();
			}
			catch (Exception x)
			{
				this.Session.Transaction.Rollback();
				throw x;
			}

			return ret;
		}

		public void Delete(T target)
		{
			this.Session.BeginTransaction();

			try
			{
				this.Session.Delete(target);
				this.Session.Transaction.Commit();
			}
			catch (Exception x)
			{
				this.Session.Transaction.Rollback();
				throw x;
			}
		}

		public List<T> Find(System.Linq.Expressions.Expression<Func<T, bool>> expression)
		{
            List<T> ret = new List<T>();

			this.Session.BeginTransaction();

			try
			{
				if (expression == null) ret = this.Session.Linq<T>().ToList();
				else ret = this.Session.Linq<T>().Where(expression).ToList();
				this.Session.Transaction.Commit();
			}
			catch (Exception x)
			{
				this.Session.Transaction.Rollback();
				throw x;
			}

			return ret;
		}

        public List<T> FindByDynamicLinq(string dynamicLinqQuery)
        {
            this.Session.BeginTransaction();

            var ret = new List<T>();

            try
            {
                if (string.IsNullOrEmpty(dynamicLinqQuery)) ret = this.Session.Linq<T>().ToList();
                else ret = this.Session.Linq<T>().Where(dynamicLinqQuery).ToList();
                this.Session.Transaction.Commit();
            }
            catch (Exception x)
            {
                this.Session.Transaction.Rollback();
                throw x;
            }

            return ret;
        }

		public ISession Session { get; set; }

		#endregion

        #region IDisposable Members

        public virtual void Dispose()
        {
            // nothing to do, just wanted to give functionality to "using" usage
        }

        #endregion
    }
}