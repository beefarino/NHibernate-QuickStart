using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using People.Domain;

namespace NHQS.Tests
{
    [TestFixture]
    public class SessionExtensionTests
    {
        [TestFixtureSetUp]
        public void setup()
        {
            SessionFactoryContainer.Current.Add(
                new PeopleSessionFactoryCreator().Create());
        }

        [Test]
        public void session_factory_can_be_obtained_by_types_it_maps()
        {
            var fac = SessionFactory.For<Person>();
            Assert.IsNotNull(fac);
        }

        [Test]
        public void session_save_extension_method_saves_instances()
        {
            var person = new Person { FirstName = "brady", LastName = "gaster" };

            SessionFactory
                .For<Person>()
                    .OpenSession()
                        .Save<Person>(person);

            Assert.Greater(person.Id, 0);
        }

        [Test]
        public void session_update_extension_method_updates_instances()
        {
            var person = new Person { FirstName = "brady", LastName = "gaster" };

            SessionFactory
                .For<Person>()
                    .OpenSession()
                        .Save<Person>(person, x =>
                            {
                                Assert.Greater(x.Id, 0);
                                x.FirstName = "Gina";
                            })
                        .Update<Person>(person, x =>
                            {
                                Assert.True(x.FirstName.Equals("Gina"));
                                Assert.True(x.Id == person.Id);
                            });
        }

        [Test]
        public void session_find_extension_method_returns_added_items()
        {
            var people = SessionFactory
                .For<Person>()
                    .OpenSession()
                        .Save<Person>(new Person { FirstName = "lucas", LastName = "gaster" })
                        .Save<Person>(new Person { FirstName = "gabriel", LastName = "gaster" })
                        .Find<Person>(x => x.LastName == "gaster");

            Assert.Greater(people.Count, 0);

            people.ForEach(x =>
                Console.WriteLine(
                    string.Format("{0} {1}", x.FirstName, x.LastName)
                    ));
        }

        [Test]
        public void session_delete_extension_method_properly_deletes()
        {
            SessionFactory
                .For<Person>()
                    .OpenSession()
                        .Save<Person>(new Person
                        {
                            FirstName = "toby",
                            LastName = "deleted"
                        }, result =>
                            {
                                Assert.That(result.Id > 0);
                            })
                        .Find<Person>(x => x.LastName == "deleted",
                            list =>
                            {
                                Assert.True(list.Count == 1);
                            })
                        .Delete<Person>(x => x.LastName == "deleted")
                        .Find<Person>(x => x.LastName == "deleted",
                            list =>
                            {
                                Assert.True(list.Count == 0);
                            });
        }

        [Test]
        public void database_actions_can_be_performed_in_a_transaction()
        {
            var s = SessionFactory.For<Person>().OpenSession();

            try
            {
                s.DoWithTransaction(session =>
                {
                    session
                        .Find<Person>(x => x.Id > 0, r => Assert.IsTrue(r.Count == 0))
                        .Save<Person>(new Person
                        {
                            FirstName = "donald",
                            LastName = "trump"
                        })
                        .Save<Person>(new Person
                        {
                            FirstName = "nene",
                            LastName = "leakes"
                        })
                        .Find<Person>(x => x.Id > 0, x => Assert.IsTrue(x.Count == 2));

                    throw new Exception("We just want to cancel the transaction");
                });
            }
            catch
            {
                s.Find<Person>(x => x.Id > 0, x => Assert.IsTrue(x.Count == 0));
            }
        }
    }
}
