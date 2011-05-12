using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.QuickStart.Configuration;
using NHibernate.QuickStart.Components;
using NHibernate.QuickStart.Implementations;

namespace NHibernate.QuickStart.DataAccess
{
	public static class SessionFactoryFactory
	{
		private static List<SessionFactoryRegistration> RegisteredSessionFactories { get; set; }
		private static ISessionFactoryContainmentService Container { get; set; }

		static SessionFactoryFactory()
		{
			SessionFactoryFactory.RegisteredSessionFactories = new List<SessionFactoryRegistration>();
		}

		private static void WrapContainmentService(ISessionFactoryContainmentService container)
		{
			if (container != null)
				SessionFactoryFactory.Container = container;
			else
			{
				if (string.IsNullOrEmpty(QuickStartConfiguration.Current.SessionFactoryContainmentService))
					SessionFactoryFactory.Container = new StaticSessionFactoryContainer();
				else
				{
					// TODO: wire it up from config
				}
			}
		}

		public static void Register(ISessionFactoryContainmentService container)
		{
			SessionFactoryFactory.WrapContainmentService(container);
			SessionFactoryFactory.RegisteredSessionFactories.Clear();

			container.Registrations.ToList().ForEach(registration =>
				{
					Register(registration);
				});
		}

		public static void Register(SessionFactoryRegistration registration)
		{
			SessionFactoryFactory.Container.Register(registration.SessionFactory, registration.Name);
			SessionFactoryFactory.RegisteredSessionFactories.Add(registration);
		}

		public static List<SessionFactoryRegistration> GetSessionFactories()
		{
			var ret = SessionFactoryFactory.RegisteredSessionFactories;

			// wire up from config
			if (QuickStartConfiguration.Configured)
			{
				if (!string.IsNullOrEmpty(QuickStartConfiguration.Current.SessionFactoryContainmentService))
				{
					ISessionFactoryContainmentService svc =
						(ISessionFactoryContainmentService)Activator.CreateInstance(
						System.Type.GetType(QuickStartConfiguration.Current.SessionFactoryContainmentService));

					SessionFactoryFactory.WrapContainmentService(svc);

					QuickStartConfiguration.Current.SessionFactories
						.Cast<SessionFactoryConfigurationElement>().ToList().ForEach(x =>
						{
							if (!ret.Any(r =>
								r.Name.Equals(x.Name, StringComparison.InvariantCultureIgnoreCase)))
							{
								ISessionFactoryRequestService rqstSvc =
									(ISessionFactoryRequestService)Activator.CreateInstance(
									System.Type.GetType(x.SessionFactoryRequestService));

								ISessionFactoryService sesFacSvc =
									(ISessionFactoryService)Activator.CreateInstance(
									System.Type.GetType(x.SessionFactoryService));

								ISessionFactory fac = sesFacSvc.Create(rqstSvc.CreateRequest());

								var reg = new SessionFactoryRegistration
								{
									Name = x.Name,
									SessionFactory = fac
								};

								SessionFactoryFactory.Register(reg);

								ret.Add(reg);
							}
						});
				}
			}

			return ret;
		}
	}

	public class SessionFactoryRegistration
	{
		public ISessionFactory SessionFactory { get; set; }
		public string Name { get; set; }
	}
}