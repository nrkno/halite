using System;
using System.Collections.Generic;
using System.Reflection;

namespace Halite.Serialization.JsonNet
{
    internal static class TypeExtensions
    {
        public static IEnumerable<Type> GetInheritanceChain(this Type type)
        {
            var t = type;
            while (t != null && t != typeof(object))
            {
                yield return t;
                t = t.BaseType;
            }
        }

        public static IEnumerable<PropertyInfo> GetImmediateProperties(this Type type)
        {
            return type.GetProperties(BindingFlags.Public
                                      | BindingFlags.Instance
                                      | BindingFlags.DeclaredOnly);
        }
    }
}