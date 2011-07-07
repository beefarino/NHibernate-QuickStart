using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Engine;
using NHibernate.Metadata;
using NHibernate.Type;

namespace NHibernate.DriveProvider.Items
{
    public class ClassMetadata : IEquatable<ClassMetadata>
    {
        private readonly IClassMetadata _metadata;

        public ClassMetadata(IClassMetadata metadata)
        {
            _metadata = metadata;
        }

        public IType GetPropertyType(string propertyName)
        {
            return _metadata.GetPropertyType(propertyName);
        }

        public object[] GetPropertyValuesToInsert(object entity, IDictionary mergeMap, ISessionImplementor session)
        {
            return _metadata.GetPropertyValuesToInsert(entity, mergeMap, session);
        }

        public System.Type GetMappedClass(EntityMode entityMode)
        {
            return _metadata.GetMappedClass(entityMode);
        }

        public object Instantiate(object id, EntityMode entityMode)
        {
            return _metadata.Instantiate(id, entityMode);
        }

        public object GetPropertyValue(object obj, string propertyName, EntityMode entityMode)
        {
            return _metadata.GetPropertyValue(obj, propertyName, entityMode);
        }

        public object[] GetPropertyValues(object entity, EntityMode entityMode)
        {
            return _metadata.GetPropertyValues(entity, entityMode);
        }

        public void SetPropertyValue(object obj, string propertyName, object value, EntityMode entityMode)
        {
            _metadata.SetPropertyValue(obj, propertyName, value, entityMode);
        }

        public void SetPropertyValues(object entity, object[] values, EntityMode entityMode)
        {
            _metadata.SetPropertyValues(entity, values, entityMode);
        }

        public object GetIdentifier(object entity, EntityMode entityMode)
        {
            return _metadata.GetIdentifier(entity, entityMode);
        }

        public void SetIdentifier(object entity, object id, EntityMode entityMode)
        {
            _metadata.SetIdentifier(entity, id, entityMode);
        }

        public bool ImplementsLifecycle(EntityMode entityMode)
        {
            return _metadata.ImplementsLifecycle(entityMode);
        }

        public bool ImplementsValidatable(EntityMode entityMode)
        {
            return _metadata.ImplementsValidatable(entityMode);
        }

        public object GetVersion(object obj, EntityMode entityMode)
        {
            return _metadata.GetVersion(obj, entityMode);
        }

        public string EntityName
        {
            get { return _metadata.EntityName; }
        }

        public string IdentifierPropertyName
        {
            get { return _metadata.IdentifierPropertyName; }
        }

        public string[] PropertyNames
        {
            get { return _metadata.PropertyNames; }
        }

        public IType IdentifierType
        {
            get { return _metadata.IdentifierType; }
        }

        public IType[] PropertyTypes
        {
            get { return _metadata.PropertyTypes; }
        }

        public bool IsMutable
        {
            get { return _metadata.IsMutable; }
        }

        public bool IsVersioned
        {
            get { return _metadata.IsVersioned; }
        }

        public int VersionProperty
        {
            get { return _metadata.VersionProperty; }
        }

        public bool[] PropertyNullability
        {
            get { return _metadata.PropertyNullability; }
        }

        public bool[] PropertyLaziness
        {
            get { return _metadata.PropertyLaziness; }
        }

        public int[] NaturalIdentifierProperties
        {
            get { return _metadata.NaturalIdentifierProperties; }
        }

        public bool IsInherited
        {
            get { return _metadata.IsInherited; }
        }

        public bool HasProxy
        {
            get { return _metadata.HasProxy; }
        }

        public bool HasIdentifierProperty
        {
            get { return _metadata.HasIdentifierProperty; }
        }

        public bool HasNaturalIdentifier
        {
            get { return _metadata.HasNaturalIdentifier; }
        }

        public bool HasSubclasses
        {
            get { return _metadata.HasSubclasses; }
        }

        public bool Equals(ClassMetadata other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other._metadata, _metadata);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (ClassMetadata)) return false;
            return Equals((ClassMetadata) obj);
        }

        public override int GetHashCode()
        {
            return (_metadata != null ? _metadata.GetHashCode() : 0);
        }

        public static bool operator ==(ClassMetadata left, ClassMetadata right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ClassMetadata left, ClassMetadata right)
        {
            return !Equals(left, right);
        }
    }
}
