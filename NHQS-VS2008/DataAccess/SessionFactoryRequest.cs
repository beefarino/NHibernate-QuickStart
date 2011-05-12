using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Conventions;
using FluentNHibernate;
using FluentNHibernate.Cfg.Db;

namespace NHibernate.QuickStart.DataAccess
{
	public class SessionFactoryRequest
	{
		public SessionFactoryRequest()
		{
			this.conventions = new List<IConvention>();
			this.createDatabase = false;
			this.logSql = false;
		}

		public SessionFactoryRequest LogSql(bool value)
		{
			this.logSql = value;
			return this;
		}

		internal Action<Cfg.Configuration> ConfigurationTweak = null;

		public SessionFactoryRequest WithConfigurationTweaks(Action<Cfg.Configuration> cfg)
		{
			this.ConfigurationTweak = cfg;
			return this;
		}

		public SessionFactoryRequest Conventions(IEnumerable<IConvention> c)
		{
			this.conventions = c;
			return this;
		}

		public SessionFactoryRequest CreateDatabase(bool value)
		{
			this.createDatabase = value;
			return this;
		}

		public SessionFactoryRequest AutoMappingSetup(Action<AutoMappingExpressions> a)
		{
			this.autoMappingSetup = a;
			return this;
		}

		public SessionFactoryRequest PersistenceConfigurer(IPersistenceConfigurer configurer)
		{
			this.persistenceConfigurer = configurer;
			return this;
		}

		internal IEnumerable<IConvention> conventions { get; set; }
		internal IPersistenceConfigurer persistenceConfigurer { get; set; }
		internal Action<AutoMappingExpressions> autoMappingSetup { get; set; }
		internal bool logSql { get; set; }
		internal bool createDatabase { get; set; }
	}
}
