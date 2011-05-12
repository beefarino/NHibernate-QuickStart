using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;

namespace NHQS
{
    public interface ISessionFactoryCreator
    {
        ISessionFactory Create();
    }
}
