using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.QuickStart.Components;
using NHibernate.QuickStart.DataAccess;

namespace NHibernate.QuickStart.Implementations
{
	public class StaticSessionFactoryContainer
		: ISessionFactoryContainmentService
	{
		public static IDictionary<string, ISessionFactory> SessionFactories { get; private set; }
		internal static object ThreadPadLock = new object();

		static StaticSessionFactoryContainer()
		{
			lock (ThreadPadLock)
			{
				StaticSessionFactoryContainer.SessionFactories =
					new Dictionary<string, ISessionFactory>();
			}
		}

		#region ISessionFactoryContainmentService Members

		public ISessionFactoryContainmentService Register(ISessionFactory sessionFactory, string name)
		{
			lock (ThreadPadLock)
			{
				StaticSessionFactoryContainer.SessionFactories.Add(name, sessionFactory);
			}

			return this;
		}

		public ISessionFactory Resolve(string name)
		{
			lock (ThreadPadLock)
			{
				return StaticSessionFactoryContainer.SessionFactories[name];
			}
		}

		public IEnumerable<SessionFactoryRegistration> Registrations
		{
			get
			{
				List<SessionFactoryRegistration> ret = new List<SessionFactoryRegistration>();
				StaticSessionFactoryContainer
					.SessionFactories
						.Keys.ToList().ForEach(key =>
							{
								ret.Add(new SessionFactoryRegistration
								{
									Name = key,
									SessionFactory = StaticSessionFactoryContainer.SessionFactories[key]
								});
							});
				return ret;
			}
		}

		#endregion
	}
}
