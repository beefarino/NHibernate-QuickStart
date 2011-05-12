using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orders.Domain
{
    public class Order
    {
        public virtual int Id { get; private set; }
        public virtual DateTime OrderDate { get; set; }
        public virtual double Total { get; set; }
    }

    public class Customer
    {
        public virtual int Id { get; private set; }
        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
    }
}
