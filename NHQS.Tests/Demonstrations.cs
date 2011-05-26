using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using People.DataAccess;
using Orders.DataAccess;
using People.Domain;

namespace NHQS.Tests
{
    [TestFixture]
    public class Demonstrations
    {
        [TestFixtureSetUp]
        public void setup()
        {
            SessionFactoryContainer.Current.Add(
                new PeopleSessionFactoryCreator().Create());

            SessionFactoryContainer.Current.Add(
                new OrdersSessionFactoryCreator().Create());

            SessionFactory
                .For<Person>()
                .OpenSession()
                    .Save<Person>(new Person
                    {
                        FirstName = "test",
                        LastName = "user"
                    });
        }

        [Test]
        public void data_was_saved()
        {
            Assert.IsTrue(
                SessionFactory
                    .For<Person>()
                    .OpenSession()
                        .Find<Person>(x => x.Id > 0)
                            .Any()
                        );
        }

        [Test]
        public void data_known_to_exist_can_be_found()
        {
            Assert.IsTrue(
                SessionFactory
                    .For<Person>()
                    .OpenSession()
                        .Find<Person>(x => x.FirstName == "test")
                            .Any()
                        );
        }

        [Test]
        public void multiple_functions_can_be_performed_fluently()
        {
            var firstName = "test";

            SessionFactory
                    .For<Person>()
                    .OpenSession()
                        .Find<Person>(x => x.FirstName == firstName,
                            people =>
                                Assert.That(people.Any()))
                        .Delete<Person>(x =>
                            x.FirstName == firstName)
                        .Find<Person>(x =>
                            x.FirstName == firstName, people =>
                                Assert.False(people.Any()));
        }
    }
}
