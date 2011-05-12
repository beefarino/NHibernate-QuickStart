using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate;
using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate.Tool.hbm2ddl;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Conventions.Helpers;
using System.Reflection;
using NHibernate.Persister.Entity;
using People.Domain;

namespace NHQS.Tests
{
    public class PeopleSessionFactoryCreator
        : ISessionFactoryCreator
    {
        public NHibernate.ISessionFactory Create()
        {
            var ret =
                Fluently
                    .Configure()
                    .Database(MsSqlCeConfiguration
                        .Standard
                        .ShowSql()
                        .FormatSql()
                        .ConnectionString(ex => 
                            ex.FromConnectionStringWithKey("PersonSqlCe")))
                    .Mappings(x => 
                        {
                            x.AutoMappings.Add(AutoMap.AssemblyOf<Person>());
                        })
                    .ExposeConfiguration(config =>
                        {
                            new SchemaExport(config).Create(true, true);
                        })
                    .BuildSessionFactory();

            return ret;
        }
    }
}
