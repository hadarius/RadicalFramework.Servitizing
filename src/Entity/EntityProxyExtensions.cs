using Castle.DynamicProxy;
using Microsoft.OData.Edm;
using Radical.Uniques;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Linq;
using Radical.Servitizing.Data.Service;

namespace Radical.Servitizing.Entity;

using Event;

public static class EntityProxyExtensions
{
    public static Type GetProxyEntityType(this Type type)
    {
        while (type.IsAssignableTo(typeof(IProxyTargetAccessor)))
            type = type.UnderlyingSystemType.BaseType;
        if (type == typeof(IEdmEntityType))
            return OpenDataServiceRegistry.Mappings[((IEdmEntityType)type).FullTypeName()];
        return type;
    }

    public static Type GetProxyEntityType(this object obj)
    {
        return obj.GetType().GetProxyEntityType();
    }

    public static string GetProxyEntityName(this object obj)
    {
        return obj.GetType().GetProxyEntityType().Name;
    }

    public static string GetProxyEntityFullName(this object obj)
    {
        return obj.GetType().GetProxyEntityType().FullName;
    }

    public static uint GetProxyEntityTypeKey32(this object obj)
    {
        return obj.GetType().GetProxyEntityType().FullName.UniqueKey32();
    }

    public static uint GetProxyEntityTypeKey32(this Type obj)
    {
        return obj.GetProxyEntityType().FullName.UniqueKey32();
    }

    public static ulong GetProxyEntityTypeKey(this object obj)
    {
        return obj.GetType().GetProxyEntityType().FullName.UniqueKey();
    }

    public static ulong GetProxyEntityTypeKey(this Type obj)
    {
        return obj.GetProxyEntityType().FullName.UniqueKey();
    }

    public static bool IsEventType(this object obj)
    {
        return obj.GetType().IsAssignableTo(typeof(Event));
    }

    public static bool IsProxyEntity(this Type t)
    {
        if (t.IsAssignableTo(typeof(IProxyTargetAccessor)))
            return false;
        if (t == typeof(IEdmEntityType))
            return false;
        return true;
    }
}