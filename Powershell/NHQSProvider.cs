using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management.Automation.Provider;
using System.Management.Automation;
using System.Linq.Expressions;
using System.Collections;
using CodeOwls.PowerShell.Paths.Processors;
using CodeOwls.PowerShell.Provider.Attributes;
using NHibernate;
using NHibernate.DriveProvider.Paths;

namespace NHQS.DriveProvider
{
    [CmdletProvider("NHQS", ProviderCapabilities.ShouldProcess)]
    public class NHQSProvider : CodeOwls.PowerShell.Provider.ProviderWithDisposableSession
    {
        class SessionContext : IDisposable
        {
            private readonly NHQSProvider _provider;

            public SessionContext( NHQSProvider provider )
            {
                _provider = provider;                
            }

            public void Dispose()
            {
                _provider.DisposeContext();
            }
        }

        private DomainPathNodeProcessor _currentPathNodeProcessor;
        private ISession _currentSession;

        protected override object NewDriveDynamicParameters()
        {
            return new DriveParameters();
        }
        
        protected override PSDriveInfo NewDrive(PSDriveInfo drive)
        {
            var prms = this.DynamicParameters as DriveParameters;
            return new NHQSDrive(drive, prms, SessionState);
        }

        protected override IPathNodeProcessor PathNodeProcessor
        {
            get
            {                
                return CurrentPathNodeProcessor;
            }
        }

        public override IDisposable NewSession()
        {
            CurrentSession.BeginTransaction();
            return new SessionContext(this);
        }

        ISession CurrentSession
        {
            get
            {
                if( null == _currentSession )
                {
                    _currentSession = Drive.NHibernateSessionFactory.OpenSession();
                }

                return _currentSession;
            }
        }

        DomainPathNodeProcessor CurrentPathNodeProcessor
        {
            get
            {
                if (null != _currentPathNodeProcessor)
                {
                    return _currentPathNodeProcessor;
                }

                _currentPathNodeProcessor = new DomainPathNodeProcessor( Drive.Name, Drive.NHibernateSessionFactory, CurrentSession );
                return _currentPathNodeProcessor;
            }
        }

        private NHQSDrive Drive
        {
            get
            {
                var drive = PSDriveInfo as NHQSDrive;

                if (null == drive)
                {
                    drive = ProviderInfo.Drives.FirstOrDefault() as NHQSDrive;
                }

                return drive;
            }
        }

        private void DisposeContext()
        {
            if( null != _currentSession )
            {
                _currentSession.Transaction.Commit();
                _currentSession.Close();
                _currentSession.Dispose();
                _currentSession = null;
            }

            _currentPathNodeProcessor = null;
        }
    }
}
