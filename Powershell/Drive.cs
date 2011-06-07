using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management.Automation;
using System.Diagnostics;
using System.Reflection;
using NHQS;
using NHibernate;

namespace NHQS.DriveProvider
{
    public class Drive<T>
        : PSDriveInfo
    {
        public DriveParameters DriveParameters { get; set; }
        public ISessionFactory NHibernateSessionFactory { get; set; }

        public Drive(PSDriveInfo driveInfo) : base(driveInfo)
        {
            this.DriveParameters = null;
        }

        public Drive(PSDriveInfo driveInfo, DriveParameters parameters) : base(driveInfo)
        {
            this.DriveParameters = parameters;
            this.CreateSessionFactoryForDrive();
        }

        private void CreateSessionFactoryForDrive()
        {
            try
            {
                this.LoadAssemblies();
                this.NHibernateSessionFactory = this.CreateSessionFactory();
            }
            catch (Exception x)
            {
                Debug.WriteLine(x);
            }
        }

        internal ISessionFactory CreateSessionFactory()
        {
            return SessionFactory.For<T>();
        }

        private void LoadAssemblies()
        {
            Assembly.LoadFrom(this.DriveParameters.DomainAssembly);
            Assembly.LoadFrom(this.DriveParameters.DataAccessAssembly);
        }
    }
}
