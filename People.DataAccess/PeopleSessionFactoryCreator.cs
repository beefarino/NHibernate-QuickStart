using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate.Tool.hbm2ddl;
using People.Domain;
using NHQS;

namespace People.DataAccess
{
    public class PeopleSessionFactoryCreator
        : ISessionFactoryCreator
    {
        public NHibernate.ISessionFactory Create()
        {
            var ret =
                Fluently
                    .Configure()
                    .Database(
                        MsSqlCeConfiguration
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
