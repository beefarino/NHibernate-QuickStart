using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using NHibernate;

namespace NHQS
{
    public class SessionFactoryContainer
    {
        public static SessionFactoryContainer Current { get; private set; }
        
        public SessionFactoryContainer LoadFromAssembly( Assembly assembly )
        {
            var types = from type in assembly.GetTypes()
                        where typeof (ISessionFactoryCreator).IsAssignableFrom(type) && type.IsClass
                        select Activator.CreateInstance( type );

            types.Cast<ISessionFactoryCreator>().ToList().ForEach(t=>Add(t.Create()));

            return this;
        }

        public SessionFactoryContainer LoadFromCurrentAppDomain()
        {
            AppDomain.CurrentDomain.GetAssemblies().ToList().ForEach( a=>LoadFromAssembly(a) );

            return this;
        }

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
