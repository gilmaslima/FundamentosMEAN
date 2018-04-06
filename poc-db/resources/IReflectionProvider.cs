using System;
using System.Reflection;
using System.Collections.Generic;

namespace Redecard.PN.Extrato.Comum
{
    delegate object MethodHandler(object target, params object[] args);

    interface IReflectionProvider
    {
        T GetSingleAttributeOrDefault<T>(MemberInfo memberInfo) where T : Attribute, new();
        IEnumerable<MemberInfo> GetSerializableMembers(Type type);
        object GetValue(MemberInfo member, object instance);
        MethodHandler GetDelegate(MethodBase method);
    }
}
