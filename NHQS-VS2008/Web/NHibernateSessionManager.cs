using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using NHibernate.QuickStart.DataAccess;
using System.Configuration;

namespace NHibernate.QuickStart.Web
{
	public class NHibernateSessionManager
	{
		public static void OpenSessions()
		{
			SessionFactoryFactory.GetSessionFactories()
				.ForEach(factoryRegistration =>
					{
						var context = HttpContext.Current;

						if (context.Items[factoryRegistration.Name] == null)
							context.Items[factoryRegistration.Name] =
								factoryRegistration.SessionFactory.OpenSession();
					});
		}

		public static void CloseSessions()
		{
			SessionFactoryFactory.GetSessionFactories()
				.ForEach(factoryRegistration =>
				{
					var context = HttpContext.Current;

					if (context.Items[factoryRegistration.Name] != null)
					{
						ISession session = context.Items[factoryRegistration.Name] as ISession;
						if (session != null)
							session.Close();
					}
				});
		}

		public static ISession GetSession(string name)
		{
			ISession session = null;
			var context = HttpContext.Current;
			if ((context != null) && (context.Items[name] != null))
				session = context.Items[name] as ISession;

			if(session == null)
				session = SessionFactoryFactory
					.GetSessionFactories()
						.First(x => x.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
							.SessionFactory
								.OpenSession();

			return session;
		}
	}
}
