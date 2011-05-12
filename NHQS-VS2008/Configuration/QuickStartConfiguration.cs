using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace NHibernate.QuickStart.Configuration
{
    public class QuickStartConfiguration : ConfigurationSection
    {
        public static QuickStartConfiguration Current
        {
			get
			{
				return ConfigurationManager.GetSection("quickStart") as QuickStartConfiguration;
			}
        }

		public static bool Configured
		{
			get
			{
				try
				{
					return (ConfigurationManager.GetSection("quickStart") 
						as QuickStartConfiguration) != null;
				}
				catch
				{
					return false;
				}
			}
		}

		private const string SessionFactoriesProperty = "sessionFactories";
		[ConfigurationProperty(SessionFactoriesProperty, IsRequired = true)]
		public GenericConfigurationCollectionElement<SessionFactoryConfigurationElement> SessionFactories
		{
			get { return (GenericConfigurationCollectionElement<SessionFactoryConfigurationElement>)base[SessionFactoriesProperty]; }
			set { base[SessionFactoriesProperty] = value; }
		}

		private const string SessionFactoryContainmentServiceProperty = "sessionFactoryContainmentService";
		[ConfigurationProperty(SessionFactoryContainmentServiceProperty, IsRequired = true)]
		public string SessionFactoryContainmentService
		{
			get { return (string)base[SessionFactoryContainmentServiceProperty]; }
			set { base[SessionFactoryContainmentServiceProperty] = value; }
		}
    }
}
