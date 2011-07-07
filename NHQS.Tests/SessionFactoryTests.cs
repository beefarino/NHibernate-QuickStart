using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using NUnit.Framework;
using People.Domain;
using People.DataAccess;

namespace NHQS.Tests
{
    [TestFixture]
    public class SessionFactoryTests
    {

        [Test]
        public void scratch()
        {
            var sessionFactory = new PeopleSessionFactoryCreator().Create();
            
            var _metadata = sessionFactory.GetClassMetadata( typeof( Person));
            var c2lassmeta = sessionFactory.GetAllClassMetadata();
            var metadata = sessionFactory.GetAllCollectionMetadata();            

            using( var session = sessionFactory.OpenSession())
            {
                var _session = session;
                
                Populate( session );

                var sessionType = _session.GetType();
                var methodInfos = from method in sessionType.GetMethods()
                                  where method.Name == "QueryOver" && method.GetParameters().Count() == 0
                                  select method;
                var typename = _metadata.GetMappedClass(EntityMode.Poco);
                var methodInfo = methodInfos.First().MakeGenericMethod(typename);
                var result = methodInfo.Invoke(_session, null);

                methodInfo = result.GetType().GetMethod("Future");
                
                result = methodInfo.Invoke(result, null);

                var e = result as IEnumerable;


            }
        }

        [Test]
        public void sample_isessionfactorycreator_creates_session_factory_instances()
        {
            var sessionFactory = new PeopleSessionFactoryCreator().Create();
            Assert.IsNotNull(sessionFactory);

            using (var session = sessionFactory.OpenSession())
            {
                Populate(session);
            }
        }

        private void Populate(ISession session)
        {
            Assert.IsNotNull(session);
            session.BeginTransaction();
            try
            {
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
