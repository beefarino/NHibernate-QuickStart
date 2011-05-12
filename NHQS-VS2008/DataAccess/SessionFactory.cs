using System;
using System.Linq;
using System.Configuration;
using FluentNHibernate;
using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate.Tool.hbm2ddl;
using FluentNHibernate.Conventions;
using System.Collections.Generic;

namespace NHibernate.QuickStart.DataAccess
{
    public static class SessionFactory
    {
        public static ISessionFactory CreateSessionFactory<TFromAutoMapAssembly>(
            SessionFactoryRequest request
            )
        {
            return CreateSessionFactory<TFromAutoMapAssembly, TFromAutoMapAssembly>(request);
        }

        public static ISessionFactory CreateSessionFactory
            <TFromAutoMapAssembly, TFromOverrideAssembly>(
            SessionFactoryRequest request
            )
        {
            var mappings = AutoMap
                .AssemblyOf<TFromAutoMapAssembly>()
                .Conventions.Add(request.conventions.ToArray())
                .UseOverridesFromAssemblyOf<TFromOverrideAssembly>();

            if (request.autoMappingSetup != null)
                mappings
                    .Setup(request.autoMappingSetup);

            return Fluently
                .Configure()
                .Database(request.persistenceConfigurer)
                .Mappings(m => m.AutoMappings.Add(mappings))
                .ExposeConfiguration(c =>
                {
					if (request.ConfigurationTweak != null)
						request.ConfigurationTweak(c);

                    new SchemaExport(c).Create(request.logSql, request.createDatabase);
                })
                .BuildSessionFactory();
        }
    }
}