using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using CodeOwls.PowerShell.Provider.PathNodeProcessors;
using NHibernate.Metadata;

namespace NHibernate.DriveProvider.Utility
{
    static class ClassMetadataExtensions
    {
        public static IQueryHelper ToQueryHelper( this IClassMetadata metadata )
        {
            var type = typeof(QueryHelper<>);
            type = type.MakeGenericType(metadata.GetMappedClass(EntityMode.Poco));

            return Activator.CreateInstance(type) as IQueryHelper;
        }

        public static object ToDynamicParameters(this IClassMetadata metadata )
        {
            return new DynamicParametersFactory(metadata).Create();
        }


        public static object ConvertDynamicParametersToPrototype(this IClassMetadata metadata, IContext context, object value)
        {
            return new DynamicParametersFactory(metadata).ConvertDynamicParametersToPrototype(context, value);
        }

        public static object ConvertDynamicParametersToPrototype(this IClassMetadata metadata, IContext context, object value, out System.Type type)
        {
            return new DynamicParametersFactory(metadata).ConvertDynamicParametersToPrototype(context, value, out type);
        }
    }
}
