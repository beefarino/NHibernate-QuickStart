using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using NHibernate.Linq;
using NHibernate.Linq.Expressions;
using System.Linq.Expressions;

namespace NHQS
{
    public static class SessionExtensions
    {
        private static ISession RealWorkWrapper(this ISession session,
            Action<ISession> realWork)
        {
            bool inLocalTransaction = true;

            if (!session.Transaction.IsActive)
                session.Transaction.Begin();
            else
                inLocalTransaction = false;

            try
            {
                realWork(session);

                if (inLocalTransaction)
                    session.Transaction.Commit();
            }
            catch
            {
                if (inLocalTransaction)
                    session.Transaction.Rollback();
            }

            return session;
        }

        public static ISession Save<T>(this ISession session, 
            T target)
            where T : class
        {
            return session.Save<T>(target, null);
        }

        public static ISession Save<T>(this ISession session, 
            T target, 
            Action<T> saveCallback)
            where T : class
        {
            return session.RealWorkWrapper(s =>
            {
                s.Save(target);

                if (saveCallback != null) 
                    saveCallback(target);
            });
        }

        public static ISession Update<T>(this ISession session, 
            T target)
            where T : class
        {
            return session.Update<T>(target, null);
        }

        public static ISession Update<T>(this ISession session, 
            T target, 
            Action<T> updateCallback)
            where T : class
        {
            return session.RealWorkWrapper(s =>
            {
                s.Update(target);

                if (updateCallback != null) 
                    updateCallback(target);
            });
        }

        public static ISession Delete<T>(this ISession session,
            T target)
            where T : class
        {
            return session.Delete<T>(target, null);
        }

        public static ISession Delete<T>(this ISession session,
            T target,
            Action<T> deleteCallback)
            where T : class
        {
            session.RealWorkWrapper(x =>
                {
                    x.Delete(target);

                    if (deleteCallback != null)
                        deleteCallback(target);
                });

            return session;
        }

        public static ISession Delete<T>(this ISession session, 
            Expression<Func<T, bool>> expression)
            where T : class
        {
            return session.Delete<T>(expression, null);
        }

        public static ISession Delete<T>(this ISession session,
            Expression<Func<T, bool>> expression,
            Action<T> deleteCallback)
            where T : class
        {
            session.RealWorkWrapper(x =>
                {
                    var toDelete = session.Find<T>(expression);
                    toDelete.ForEach(d =>
                        {
                            x.Delete(d);

                            if (deleteCallback != null)
                                deleteCallback(d);
                        });
                });

            return session;
        }

        public static List<T> Find<T>(this ISession session,
            Expression<Func<T, bool>> expression) 
            where T : class
        {
            List<T> ret = new List<T>();

            bool inLocalTransaction = true;

            if (!session.Transaction.IsActive)
                session.Transaction.Begin();
            else
                inLocalTransaction = false;

            try
            {
                ret = session.Query<T>().Where<T>(expression).ToList();

                if (inLocalTransaction)
                    session.Transaction.Commit();
            }
            catch
            {
                if (inLocalTransaction)
                    session.Transaction.Rollback();
            }

            return ret;
        }

        public static ISession Find<T>(this ISession session,
            Expression<Func<T, bool>> expression,
            Action<IList<T>> resultListCallback)
            where T : class
        {
            List<T> ret = session.Find<T>(expression);
            if (resultListCallback != null)
                resultListCallback(ret);
            return session;
        }

        public static List<T> GetAll<T>(this ISession session)
            where T : class
        {
            return session.Find<T>(t=>true);
        }

        public static ISession DoWithTransaction(this ISession session,
            Action<ISession> unitsOfWork)
        {
            session.BeginTransaction();

            try
            {
                unitsOfWork(session);
                session.Transaction.Commit();
            }
            catch(Exception x)
            {
                session.Transaction.Rollback();
                throw x;
            }

            return session;
        }
    }
}