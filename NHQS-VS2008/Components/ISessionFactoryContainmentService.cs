using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.QuickStart.DataAccess;

namespace NHibernate.QuickStart.Components
{
	public interface ISessionFactoryContainmentService
	{
		ISessionFactoryContainmentService Register(ISessionFactory sessionFactory, string name);
		ISessionFactory Resolve(string name);
		IEnumerable<SessionFactoryRegistration> Registrations { get; }
	}
}