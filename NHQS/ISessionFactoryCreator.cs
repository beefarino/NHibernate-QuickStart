using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHQS
{
    public interface ISessionFactoryCreator
    {
        NHibernate.ISessionFactory Create();
    }
}
