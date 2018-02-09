using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Halite.Serialization.JsonNet
{
    public class HalResourceJsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var objectType = value.GetType();
            var jo = new JObject();

            var properties = objectType.GetInheritanceChain()
                .Reverse()
                .SelectMany(it => it.GetImmediateProperties())
                .ToList();

            var linksProperty = properties.Single(p => string.Equals("Links", p.Name));
            var embeddedProperty = properties.SingleOrDefault(p => string.Equals("Embedded", p.Name));
            var halResourceProperties = new[] {linksProperty, embeddedProperty}.Where(it => it != null);

            var regularProperties = properties.Except(halResourceProperties);

            AddPropertyValue(jo, "_links", linksProperty, value, serializer);

            foreach (var prop in regularProperties.Where(p => p.CanRead))
            {
                AddPropertyValue(jo, prop.GetPropertyName(), prop, value, serializer);
            }

            if (embeddedProperty != null)
            {
                AddPropertyValue(jo, "_embedded", embeddedProperty, value, serializer);
            }

            jo.WriteTo(writer);
        }

        private static void AddPropertyValue(JObject jo, string name, PropertyInfo prop, object value, JsonSerializer serializer)
        {
            var propVal = prop.GetValue(value, null);
            if (propVal != null)
            {
                jo.Add(name, JToken.FromObject(propVal, serializer));
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.StartObject)
            {
                var jo = JObject.Load(reader);

                var ctor = SelectConstructor(objectType);
                if (ctor == null)
                {
                    throw CreateConstructorException(objectType);
                }

                var instance = CreateInstance(objectType, ctor, jo, serializer);

                AssignValues(objectType, instance, jo, serializer);

                return instance;
            }

            throw new InvalidOperationException();
        }

        private static JsonSerializationException CreateConstructorException(Type objectType)
        {
            return new JsonSerializationException($"Unable to find a constructor to use for type {objectType}. A class should either have a default constructor, one constructor with arguments or a constructor marked with the JsonConstructor attribute.");
        }

        private static void AssignValues(Type objectType, object instance, JObject jo, JsonSerializer serializer)
        {
            var properties = objectType.GetProperties().Where(p => p.SetMethod != null && p.GetMethod != null).ToList();

            foreach (var prop in properties)
            {
                var jop = jo.Properties().FirstOrDefault(it =>
                    string.Equals(it.Name, prop.GetPropertyName(), StringComparison.InvariantCultureIgnoreCase));
                if (jop != null)
                {
                    var currentValue = prop.GetMethod.Invoke(instance, new object[0]);
                    if (currentValue == null)
                    {
                        var value = jop.Value.ToObject(prop.PropertyType, serializer);
                        prop.SetMethod.Invoke(instance, new[] { value });
                    }
                }
            }
        }

        private static object CreateInstance(Type objectType, ConstructorInfo ctor, JObject item, JsonSerializer serializer)
        {
            var args = ctor.GetParameters().Select(p => LookupArgument(objectType, p, item, serializer)).ToArray();
            try
            {
                return ctor.Invoke(args);
            }
            catch (TargetInvocationException ex)
            {
                if (ex.InnerException != null)
                {
                    throw ex.InnerException;
                }

                throw;
            }
        }

        private static object LookupArgument(Type objectType, ParameterInfo parameter, JObject item, JsonSerializer serializer)
        {
            var prop = FindCorrespondingProperty(objectType, parameter.Name);
            if (prop == null)
            {
                throw CreateConstructorException(objectType);
            }

            var jprop = item.Properties().FirstOrDefault(it => string.Equals(prop.GetPropertyName(), it.Name, StringComparison.InvariantCultureIgnoreCase));
            if (jprop == null)
            {
                throw CreateConstructorException(objectType);
            }

            var val = jprop.Value.ToObject(parameter.ParameterType, serializer);
            return val;
        }

        private static PropertyInfo FindCorrespondingProperty(Type objectType, string parameterName)
        {
            return objectType.GetProperties().FirstOrDefault(it =>
                string.Equals(it.Name, parameterName, StringComparison.InvariantCultureIgnoreCase));
        }


        private static ConstructorInfo SelectConstructor(Type objectType)
        {
            var constructors = objectType.GetConstructors();

            return SelectAnnotatedJsonConstructor(constructors) ??
                   SelectDefaultConstructor(constructors) ??
                   SelectConstructorWithParameters(constructors);
        }

        private static ConstructorInfo SelectAnnotatedJsonConstructor(IReadOnlyList<ConstructorInfo> constructors)
        {
            return constructors.SingleOrDefault(ctor => ctor.GetCustomAttributes(typeof(JsonConstructorAttribute), false).Any());
        }

        private static ConstructorInfo SelectDefaultConstructor(IReadOnlyList<ConstructorInfo> constructors)
        {
            return constructors.SingleOrDefault(ctor => !ctor.GetParameters().Any());
        }

        private static ConstructorInfo SelectConstructorWithParameters(IReadOnlyList<ConstructorInfo> ctors)
        {
            return ctors.Count == 1 ? ctors[0] : null;
        }

        public override bool CanConvert(Type objectType)
        {
            var res = IsHalResourceType(objectType);
            return res;
        }

        private static bool IsHalResourceType(Type type)
        {
            return type != null &&
                   (IsExactlyHalResourceType(type) || IsHalResourceType(type.BaseType));
        }

        private static bool IsExactlyHalResourceType(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(HalResource<>);
        }
    }
}