using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;

namespace NHQS
{
    public class SessionFactoryContainer
    {
        public static SessionFactoryContainer Current { get; private set; }
        internal List<ISessionFactory> SessionFactories { get; private set; }
        private static object padlock = new object();

        static SessionFactoryContainer()
        {
            lock (padlock)
            {
                SessionFactoryContainer.Current = new SessionFactoryContainer();
            }
        }

        private SessionFactoryContainer()
        {
            this.SessionFactories = new List<ISessionFactory>();
        }

        public SessionFactoryContainer Add(ISessionFactory sessionFactory)
        {
            lock (padlock)
            {
                SessionFactoryContainer.Current.SessionFactories.Add(sessionFactory);
                return SessionFactoryContainer.Current;
            }
        }
    }
}
