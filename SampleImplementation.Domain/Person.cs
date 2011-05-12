using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SampleImplementation.Domain
{
    public class Person
    {
        public virtual int Id { get; private set; }
        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
    }
}
