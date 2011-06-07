using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management.Automation.Provider;
using System.Management.Automation;
using System.Linq.Expressions;
using System.Collections;
using NHibernate;

namespace NHQS.DriveProvider
{
    [CmdletProvider("NHibernate", ProviderCapabilities.Filter)]
    public class Provider
        : ContainerCmdletProvider
    {
        // runs first
        protected override ProviderInfo Start(ProviderInfo providerInfo)
        {
            return base.Start(providerInfo);
        }

        // runs second
        protected override object NewDriveDynamicParameters()
        {
            return new DriveParameters();
        }

        // runs third
        protected override PSDriveInfo NewDrive(PSDriveInfo drive)
        {
            var prms = this.DynamicParameters as DriveParameters;
            return new Drive(drive, prms);
        }

        protected override bool IsValidPath(string path)
        {
            return true;
        }

        protected override bool ItemExists(string path)
        {
            return true;
        }

        protected override void NewItem(string path, string itemTypeName, object newItemValue)
        {
            if (newItemValue is PSObject) newItemValue = ((PSObject)newItemValue).BaseObject;
            Drive nhibernateDrive = this.PSDriveInfo as Drive;

            using (ISession session = nhibernateDrive.NHibernateSessionFactory.OpenSession())
            {
                // get the type of object identified in the path parameter
                var domainType = newItemValue.GetType();

                // find the extension method to call
                var method = typeof(NHibernate.QuickStart.Repositories.NHibernateSessionExtensions)
                    .GetMethod("CreateRepository");

                var generic = method.MakeGenericMethod(domainType);

                // create the generic repository for this type of object
                var repository = generic.Invoke(null, new object[] { session });

                // add the new item to the database
                newItemValue = repository
                    .GetType()
                    .GetMethod("Add")
                    .Invoke(repository, new object[] { newItemValue });

                // write the objects
                WriteItemObject(newItemValue, path, false);
            }
        }

        protected override void GetItem(string path)
        {
            Drive nhibernateDrive = this.PSDriveInfo as Drive;

            using (ISession session = nhibernateDrive.NHibernateSessionFactory.OpenSession())
            {
                // get the type of object identified in the path parameter
                var tpNm = path;
                var domainType = System.Type.GetType(tpNm);

                // find the extension method to call
                var method = typeof(NHibernate.QuickStart.Repositories.NHibernateSessionExtensions)
                    .GetMethod("CreateRepository");

                var generic = method.MakeGenericMethod(domainType);

                // create the generic repository for this type of object
                var repository = generic.Invoke(null, new object[] { session });

                // get the filtered list as a dynamic linq query
                var results = repository
                    .GetType()
                    .GetMethod("FindByDynamicLinq")
                    .Invoke(repository, new object[] { this.Filter });

                // write the objects
                WriteItemObject(results, path, false);
            }
        }

        protected override void CopyItem(string path, string copyPath, bool recurse)
        {
            object target = null;

            Drive sourceDrive = this.PSDriveInfo as Drive;
            var domainType = System.Type.GetType(path);

            using (ISession session = sourceDrive.NHibernateSessionFactory.OpenSession())
            {
                // find the extension method to call
                var method = typeof(NHibernate.QuickStart.Repositories.NHibernateSessionExtensions)
                    .GetMethod("CreateRepository");

                var generic = method.MakeGenericMethod(domainType);

                // create the generic repository for this type of object
                var repository = generic.Invoke(null, new object[] { session });

                // get the filtered list as a dynamic linq query
                var results = repository
                    .GetType()
                    .GetMethod("FindByDynamicLinq")
                    .Invoke(repository, new object[] { this.Filter });

                target = results;
            }

            Drive destinationDrive = this.ProviderInfo.Drives.First(x =>
                x.Name == DestinationDriveName) as Drive;

            using (ISession session = destinationDrive.NHibernateSessionFactory.OpenSession())
            {
                // find the extension method to call
                var method = typeof(NHibernate.QuickStart.Repositories.NHibernateSessionExtensions)
                    .GetMethod("CreateRepository");

                var generic = method.MakeGenericMethod(domainType);

                // create the generic repository for this type of object
                var repository = generic.Invoke(null, new object[] { session });

                // if this is a list to be inserted, iterate through it
                var genericListTyp = typeof(List<>).MakeGenericType(domainType);
                if (genericListTyp.Equals(target.GetType()))
                {
                    foreach (var targetItem in (IEnumerable)target)
                    {
                        // add the new item to the database
                        repository
                            .GetType()
                            .GetMethod("Add")
                            .Invoke(repository, new object[] { targetItem });
                    }
                }
                else
                {
                    // add the new item to the database
                    target = repository
                        .GetType()
                        .GetMethod("Add")
                        .Invoke(repository, new object[] { target });
                }
            }
        }

        static string DestinationDriveName { get; set; } 

        protected override object CopyItemDynamicParameters(string path, string destination, bool recurse)
        {
            DestinationDriveName = destination.Substring(0, destination.IndexOf(':'));

            return null;
        }
    }
}
