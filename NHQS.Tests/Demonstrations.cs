using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using People.DataAccess;
using Orders.DataAccess;

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
        }
    }
}
