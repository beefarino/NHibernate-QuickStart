using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using People.Domain;
using People.DataAccess;

namespace NHQS.Tests
{
    [TestFixture]
    public class TransactionTests
    {
        [TestFixtureSetUp]
        public void setup()
        {
            SessionFactoryContainer.Current.Add(
                new PeopleSessionFactoryCreator().Create());
        }

        [Test]
        public void transaction_can_be_managed_external_to_repository_methods()
        {
            using (var session = SessionFactory.For<Person>().OpenSession())
            {
                session.BeginTransaction();

                try
                {
                    session
                        .Save<Person>(new Person
                        {
                            FirstName = "lucas",
                            LastName = "gaster"
                        })
                        .Save<Person>(new Person
                        {
                            FirstName = "gabriel",
                            LastName = "gaster"
                        });

                    session.Transaction.Commit();
                }
                catch
                {
                    session.Transaction.Rollback();
                }

                Assert.Greater(session.Find<Person>(x => x.LastName == "gaster").Count, 0);
            }
        }
    }
}
