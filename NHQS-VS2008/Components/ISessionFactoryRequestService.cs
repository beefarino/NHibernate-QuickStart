using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.QuickStart.DataAccess;

namespace NHibernate.QuickStart.Components
{
	public interface ISessionFactoryRequestService
	{
		SessionFactoryRequest CreateRequest();
	}
}