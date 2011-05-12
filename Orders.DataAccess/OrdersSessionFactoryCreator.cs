using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHQS;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Automapping;
using Orders.Domain;
using NHibernate.Tool.hbm2ddl;

namespace Orders.DataAccess
{
    public class OrdersSessionFactoryCreator
        : ISessionFactoryCreator
    {
        #region ISessionFactoryCreator Members

        public NHibernate.ISessionFactory Create()
        {
            var ret =
                Fluently
                    .Configure()
                    .Database(MsSqlConfiguration
                        .MsSql2008
                        .ShowSql()
                        .FormatSql()
                        .ConnectionString(ex =>
                            ex.FromConnectionStringWithKey("OrdersSqlServer")))
                    .Mappings(x =>
                    {
                        x.AutoMappings.Add(AutoMap.AssemblyOf<Order>());
                    })
                    .ExposeConfiguration(config =>
                    {
                        new SchemaExport(config).Create(true, true);
                    })
                    .BuildSessionFactory();

            return ret;
        }

        #endregion
    }
}
