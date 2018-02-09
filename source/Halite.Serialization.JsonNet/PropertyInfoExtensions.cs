using System.Reflection;
using Newtonsoft.Json;

namespace Halite.Serialization.JsonNet
{
    internal static class PropertyInfoExtensions
    {
        public static string GetRelationName(this PropertyInfo prop)
        {
            var attribute = prop.GetCustomAttribute(typeof(HalRelationAttribute)) as HalRelationAttribute;
            return attribute == null ? prop.Name : attribute.Name;
        }

        public static string GetPropertyName(this PropertyInfo prop)
        {
            var attribute = prop.GetCustomAttribute(typeof(HalPropertyAttribute)) as HalPropertyAttribute;
            return attribute == null ? prop.Name : attribute.Name;
        }
    }
}