using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Conventions;

namespace NHibernate.QuickStart.Conventions
{
	public class NaiveConventions : 
		IReferenceConvention,
		IHasManyConvention,
		IHasManyToManyConvention
	{
		#region IConvention<IManyToOneInspector,IManyToOneInstance> Members

		public void Apply(FluentNHibernate.Conventions.Instances.IManyToOneInstance instance)
		{
			instance.Cascade.All();
		}

		#endregion

		#region IConvention<IOneToManyCollectionInspector,IOneToManyCollectionInstance> Members

		public void Apply(FluentNHibernate.Conventions.Instances.IOneToManyCollectionInstance instance)
		{
			instance.Cascade.All();
		}

		#endregion

		#region IConvention<IManyToManyCollectionInspector,IManyToManyCollectionInstance> Members

		public void Apply(FluentNHibernate.Conventions.Instances.IManyToManyCollectionInstance instance)
		{
			instance.Cascade.All();
		}

		#endregion
	}

}
