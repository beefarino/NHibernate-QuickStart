using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace NHibernate.QuickStart.Configuration
{
	public class GenericConfigurationCollectionElement<T> : ConfigurationElementCollection
		where T : ConfigurationElement, new()
	{
		public override ConfigurationElementCollectionType CollectionType
		{
			get
			{
				return ConfigurationElementCollectionType.AddRemoveClearMap;
			}
		}

		protected override ConfigurationElement CreateNewElement()
		{
			return new T();
		}

		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((T)element).GetHashCode(); // this could probably be improved upon
		}
	}
}
