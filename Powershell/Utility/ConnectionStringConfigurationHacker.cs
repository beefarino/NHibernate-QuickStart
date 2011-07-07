using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace NHQS.DriveProvider.Utility
{
    class ConnectionStringConfigurationHacker
    {
        private readonly ConnectionStringSettingsCollection _connectionStrings;

        public ConnectionStringConfigurationHacker()
        {
            _connectionStrings = ConfigurationManager.ConnectionStrings;

            var fi = typeof(ConfigurationElementCollection)
                .GetField(
                    "bReadOnly",
                    System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic
                );

            fi.SetValue(_connectionStrings, false);            
        }

        public ConnectionStringConfigurationHacker Add( string connectionStringName, string connectionString )
        {
            _connectionStrings.Add(
                new ConnectionStringSettings(
                    connectionStringName,
                    connectionString
                )
            );

            return this;
        }

        public ConnectionStringConfigurationHacker FromConfigFile( string configFilePath )
        {
            var document = XDocument.Load(configFilePath);
            var connectionStrings = from xe in document.Root.Descendants("connectionStrings").Descendants("add")
                                    select
                                        new
                                            {
                                                Name = xe.Attribute("name").Value,
                                                ConnectionString = xe.Attribute("connectionString").Value
                                            };

            connectionStrings.ToList().ForEach(o => Add(o.Name, o.ConnectionString));

            return this;
        }
    }
}
