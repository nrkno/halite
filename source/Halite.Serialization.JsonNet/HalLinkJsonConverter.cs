using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Halite.Serialization.JsonNet
{
    public class HalLinkJsonConverter : JsonConverter
    {
        private static JProperty CreateProperty(string name, object value)
        {
            return value == null ? null : new JProperty(name, JToken.FromObject(value));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var link = (HalLinkObject) value;

            var maybeProperties = new object[]
            {
                CreateProperty("name", link.Name),
                CreateProperty("href", link.Href),
                CreateProperty("templated", link.Templated),
                CreateProperty("type", link.Type),
                CreateProperty("deprecation", link.Deprecation),
                CreateProperty("profile", link.Profile),
                CreateProperty("title", link.Title),
                CreateProperty("hreflang", link.HrefLang)
            };

            var properties = maybeProperties.Where(it => it != null).ToArray();
            var jo = new JObject(properties);
            jo.WriteTo(writer);
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

                var instance = CreateInstance(objectType, ctor, jo);

                AssignValues(objectType, instance, jo);

                return instance;
            }

            throw new InvalidOperationException();
        }

        private static JsonSerializationException CreateConstructorException(Type objectType)
        {
            return new JsonSerializationException($"Unable to find a constructor to use for type {objectType}. A class should either have a default constructor, one constructor with arguments or a constructor marked with the JsonConstructor attribute.");
        }

        private static void AssignValues(Type objectType, HalLinkObject instance, JObject jo)
        {
            var properties = objectType.GetProperties().Where(p => p.SetMethod != null && p.GetMethod != null).ToList();

            foreach (var prop in properties)
            {
                var jop = jo.Properties().FirstOrDefault(it =>
                    string.Equals(it.Name, prop.Name, StringComparison.InvariantCultureIgnoreCase));
                if (jop != null)
                {
                    var currentValue = prop.GetMethod.Invoke(instance, new object[0]);
                    if (currentValue == null)
                    {
                        var jvalue = (JValue)jop.Value;
                        var objValue = jvalue.Value;
                        var value = typeof(Uri) == prop.PropertyType
                            ? new Uri((string)objValue, UriKind.RelativeOrAbsolute)
                            : objValue;
                        prop.SetMethod.Invoke(instance, new[] { value });
                    }
                }
            }
        }

        private static HalLinkObject CreateInstance(Type objectType, ConstructorInfo ctor, JObject item)
        {
            var args = ctor.GetParameters().Select(p => LookupArgument(objectType, p, item)).ToArray();
            try
            {
                return (HalLinkObject) ctor.Invoke(args);
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

        private static object LookupArgument(Type objectType, ParameterInfo parameter, JObject item)
        {
            var property = item.Properties().FirstOrDefault(prop => string.Equals(parameter.Name, prop.Name, StringComparison.InvariantCultureIgnoreCase));
            if (property == null)
            {
                throw CreateConstructorException(objectType);
            }

            var jval = (JValue) property.Value;
            var val = jval?.Value;
            if (val != null)
            {
                if (!parameter.ParameterType.IsInstanceOfType(val))
                {
                    throw CreateConstructorException(objectType);
                }
            }

            return val;
        }

        private static ConstructorInfo SelectConstructor(Type objectType)
        {
            return SelectHalLinkConstructor(objectType) ??
                   SelectSubclassConstructor(objectType);
        }

        private static ConstructorInfo SelectHalLinkConstructor(Type objectType)
        {
            if (objectType == typeof(HalLink) || objectType == typeof(HalTemplatedLink))
            {
                return objectType.GetConstructors().Single(AcceptsSingleStringParameter);
            }

            return null;
        }

        private static bool AcceptsSingleStringParameter(ConstructorInfo ctor)
        {
            var parameters = ctor.GetParameters();
            return parameters.Length == 1 && parameters[0].ParameterType == typeof(string);
        }

        private static ConstructorInfo SelectSubclassConstructor(Type objectType)
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
            var result = typeof(HalLinkObject).IsAssignableFrom(objectType);
            return result;
        }
    }
}
