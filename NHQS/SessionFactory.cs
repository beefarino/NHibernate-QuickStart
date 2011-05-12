using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using NHibernate.Persister.Entity;

namespace NHQS
{
    public static class SessionFactory
    {
        public static ISessionFactory For<T>()
        {
            ISessionFactory ret = null;

            SessionFactoryContainer.Current.SessionFactories.ForEach(factory =>
                {
                    if (ret == null)
                    {
                        factory
                            .GetAllClassMetadata()
                            .ToList()
                                .ForEach(keyValuePair =>
                                {
                                    AbstractEntityPersister val =
                                        keyValuePair.Value as AbstractEntityPersister;

                                    if (val.EntityType.ReturnedClass.Equals(typeof(T)))
                                        ret = factory;
                                });
                    }
                });

            return ret;
        }
    }
}
