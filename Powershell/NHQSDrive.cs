using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management.Automation;
using System.Diagnostics;
using System.Reflection;
using System.Text.RegularExpressions;
using CodeOwls.PowerShell.Paths.Processors;
using CodeOwls.PowerShell.Provider.PathNodes;
using NHibernate.DriveProvider.Paths;
using NHQS;
using NHibernate;
using NHQS.DriveProvider.Utility;

namespace NHQS.DriveProvider
{
    public class NHQSDrive : CodeOwls.PowerShell.Provider.Drive
    {
        private Type _domainObjectType;
        
        public NHQSDrive(PSDriveInfo driveInfo, DriveParameters parameters, SessionState sessionState) 
            : base(driveInfo)
        {
            this.DriveParameters = parameters;
            CreateSessionFactoryForDrive();
        }

        public ISessionFactory NHibernateSessionFactory { get; private set; }

        DriveParameters DriveParameters
        {
            get; set;
        }

        private void CreateSessionFactoryForDrive()
        {
            try
            {
                this.LoadConnectionStrings();
                this.LoadAssemblies();
                this.NHibernateSessionFactory = this.CreateSessionFactory();
            }
            catch (Exception x)
            {
                Debug.WriteLine(x);
            }
        }

        private void LoadConnectionStrings()
        {
            var hacker = new ConnectionStringConfigurationHacker();
            if( ! String.IsNullOrEmpty( DriveParameters.ConfigurationFile ) )
            {
                hacker.FromConfigFile(DriveParameters.ConfigurationFile);
            }

            if( null != DriveParameters.ConnectionString )
            {
                var cxns = DriveParameters.ConnectionString;
                foreach (string key in cxns.Keys)
                {
                    hacker.Add(key, (string)cxns[key]);
                }
            }
        }

        private ISessionFactory CreateSessionFactory()
        {
            if (null == _domainObjectType)
            {
                LoadAssemblies();
            }
            return SessionFactory.For(_domainObjectType);
        }

        private void LoadAssemblies()
        {
            if (!DriveParameters.DomainAssembly.EndsWith(".dll", StringComparison.InvariantCultureIgnoreCase))
            {
                var root = DriveParameters.DomainAssembly.Split('.').First();
                _domainObjectType = (from asm in AppDomain.CurrentDomain.GetAssemblies()
                                     where asm.FullName.StartsWith(root)
                                     from type in asm.GetTypes()
                                     where StringComparer.InvariantCultureIgnoreCase.Equals(
                                         type.FullName, DriveParameters.DomainAssembly
                                         )
                                     select type).FirstOrDefault();
            }
            if (null == _domainObjectType)
            {
                var assemblyName = DriveParameters.DomainAssembly;
                var domainAssembly = Assembly.LoadFrom(assemblyName);

                var domainTypes = from type in domainAssembly.GetTypes()
                                  where type.BaseType == typeof (object)
                                  select type;
                _domainObjectType = domainTypes.First();
            }

            if (null != DriveParameters.DataAccessAssembly)
            {
                var dataAccessAssembly = Assembly.LoadFrom(DriveParameters.DataAccessAssembly);
                SessionFactoryContainer.Current.LoadFromAssembly(dataAccessAssembly);
            }
            else
            {
                SessionFactoryContainer.Current.LoadFromCurrentAppDomain();
            }

        }

    }
}
