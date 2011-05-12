using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace NHibernate.QuickStart.Configuration
{
	public class SessionFactoryConfigurationElement : ConfigurationElement
	{
		private const string NameProperty = "name";
		[ConfigurationProperty(NameProperty, IsRequired = true)]
		public string Name
		{
			get { return (string)base[NameProperty]; }
			set { base[NameProperty] = value; }
		}

		private const string TypeProperty = "sessionFactoryRequestService";
		[ConfigurationProperty(TypeProperty, IsRequired = true)]
		public string SessionFactoryRequestService
		{
			get { return (string)base[TypeProperty]; }
			set { base[TypeProperty] = value; }
		}

		private const string SessionFactoryServiceProperty = "sessionFactoryService";
		[ConfigurationProperty(SessionFactoryServiceProperty, IsRequired = true)]
		public string SessionFactoryService
		{
			get { return (string)base[SessionFactoryServiceProperty]; }
			set { base[SessionFactoryServiceProperty] = value; }
		}
	}
}
