using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Redecard.PN.Extrato.Comum
{
    abstract class BaseReflector : IReflectionProvider
    {
        public virtual T GetSingleAttributeOrDefault<T>(MemberInfo memberInfo) where T : Attribute, new()
        {
            object[] attributes = memberInfo.GetCustomAttributes(typeof(T), false);
            return attributes.Length == 0 ? new T() : attributes[0] as T;
        }

        public virtual IEnumerable<MemberInfo> GetSerializableMembers(Type type)
        {
            return type.GetProperties(ReflectionHelper.PublicInstanceMembers | BindingFlags.FlattenHierarchy)
                .Where(p => p.GetGetMethod() != null && p.GetGetMethod().GetParameters().Length == 0)
                .Cast<MemberInfo>()
                .Union(type.GetFields(ReflectionHelper.PublicInstanceMembers | BindingFlags.FlattenHierarchy).Cast<MemberInfo>());
        }

        public abstract object GetValue(MemberInfo member, object instance);
        public abstract MethodHandler GetDelegate(MethodBase method);
    }
}
