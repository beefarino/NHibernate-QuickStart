using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Orders.DataAccess;
using People.Domain;
using Orders.Domain;
using NHibernate;
using People.DataAccess;

namespace NHQS.Tests
{
    [TestFixture]
    public class MultipleDatabaseTests
    {
        public ISessionFactory PeopleDatabase { get; set; }
        public ISessionFactory OrdersDatabase { get; set; }

        [TestFixtureSetUp]
        public void setup()
        {
            SessionFactoryContainer
                .Current
                    .Add(new PeopleSessionFactoryCreator().Create())
                    .Add(new OrdersSessionFactoryCreator().Create());

            SessionFactory
                .For<Person>()
                    .OpenSession()
                        .Save<Person>(new Person
                        {
                            FirstName = "paul",
                            LastName = "sebastian"
                        }, x => Assert.IsTrue(x.Id > 0))
                        .Save<Person>(new Person
                        {
                            FirstName = "sebastian",
                            LastName = "bach"
                        }, x => Assert.IsTrue(x.Id > 0));

            SessionFactory
                .For<Order>()
                    .OpenSession()
                        .Save<Order>(new Order
                        {
                            OrderDate = DateTime.Now,
                            Total = 29.99
                        })
                        .Save<Order>(new Order
                        {
                            OrderDate = DateTime.Now.AddMinutes(-5),
                            Total = 19.99
                        })
                        .Save<Order>(new Order
                        {
                            OrderDate = DateTime.Now.AddMinutes(-4),
                            Total = 9.99
                        });
        }

        [Test]
        public void sample_data_is_added_properly()
        {
            SessionFactory.For<Person>().OpenSession().Find<Person>(x => x.Id > 0, x => Assert.IsTrue(x.Count > 0));
            SessionFactory.For<Order>().OpenSession().Find<Order>(x => x.Id > 0, x => Assert.IsTrue(x.Count > 0));
        }

        [Test]
        public void sample_data_can_be_copied_between_databases()
        {
            SessionFactory
                .For<Person>()
                    .OpenSession()
                        .Find<Person>(x => x.Id > 0, x =>
                            {
                                x.ToList().ForEach(person =>
                                    {
                                        SessionFactory
                                            .For<Customer>()
                                                .OpenSession()
                                                    .Save<Customer>(
                                                        new Customer
                                                        {
                                                            FirstName = person.FirstName,
                                                            LastName = person.LastName
                                                        }, c => Assert.IsTrue(c.Id > 0));
                                    });
                            });
        }
    }
}
