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
    public class SessionFactoryTests
    {
        [Test]
        public void sample_isessionfactorycreator_creates_session_factory_instances()
        {
            var sessionFactory = new PeopleSessionFactoryCreator().Create();
            Assert.IsNotNull(sessionFactory);

            using (var session = sessionFactory.OpenSession())
            {
                session.BeginTransaction();

                try
                {
                    Assert.IsNotNull(session);

                    var person = new Person
                    {
                        FirstName = "brady",
                        LastName = "gaster"
                    };

                    session.Save(person);
                    Assert.Greater(person.Id, 0);

                    int newPersonId = person.Id;

                    person.FirstName = "Gina";
                    session.Update(person);

                    Assert.Greater(person.Id, 0);
                    Assert.IsTrue(person.Id == newPersonId);

                    session.Transaction.Commit();
                }
                catch
                {
                    session.Transaction.Rollback();
                }
            }
        }
    }
}
