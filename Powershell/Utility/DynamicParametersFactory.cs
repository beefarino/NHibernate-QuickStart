using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using CodeOwls.PowerShell.Provider.PathNodeProcessors;
using NHibernate.Metadata;
using System.Collections.ObjectModel;

namespace NHibernate.DriveProvider.Utility
{
    class DynamicParametersFactory
    {
        private readonly IClassMetadata _metadata;

        public DynamicParametersFactory( IClassMetadata metadata )
        {
            _metadata = metadata;
        }

        public object Create()
        {
            var type = _metadata.GetMappedClass(EntityMode.Poco);

            var parameters = new RuntimeDefinedParameterDictionary();

            var attrs = from prop in type.GetProperties()
                        // where prop.Name != _metadata.IdentifierPropertyName
                        select new RuntimeDefinedParameter(
                            prop.Name,
                            prop.PropertyType,
                            new Collection<Attribute>
                                    {
                                        new ParameterAttribute()
                                    }
                            );
            attrs.ToList().ForEach(attr => parameters.Add(attr.Name, attr));
            return parameters;
        }

        public object ConvertDynamicParametersToPrototype(IContext context, object value)
        {
            System.Type type;
            return ConvertDynamicParametersToPrototype(context, value, out type);
        }

        public object ConvertDynamicParametersToPrototype(IContext context, object value, out System.Type type)
        {
            type = _metadata.GetMappedClass(EntityMode.Poco);
            var localType = type;
            
            if (null != value )
            {
                var pso = PSObject.AsPSObject(value);
                if (pso.TypeNames.Contains(type.FullName))
                {
                    return Convert.ChangeType(pso.BaseObject, type);
                }
            }

            var prototype = Activator.CreateInstance(type);
            var psoParams = (RuntimeDefinedParameterDictionary)context.DynamicParameters;
            psoParams.Values.ToList().ForEach(p =>
            {
                var pi = localType.GetProperty(p.Name);
                pi.SetValue(prototype, p.Value, null);
            });
            return prototype;
        }

    }
}
