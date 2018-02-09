using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Halite.Serialization.JsonNet
{
    public static class SerializerConfigExtensions
    {
        public static JsonSerializerSettings ConfigureForHalite(this JsonSerializerSettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            foreach (var c in GetHaliteConverters())
            {
                settings.Converters.Add(c);
            }

            return settings;
        }

        public static JsonSerializer ConfigureForHalite(this JsonSerializer serializer)
        {
            if (serializer == null)
            {
                throw new ArgumentNullException(nameof(serializer));
            }

            foreach (var c in GetHaliteConverters())
            {
                serializer.Converters.Add(c);
            }

            return serializer;
        }

        private static IEnumerable<JsonConverter> GetHaliteConverters()
        {
            yield return new HalLinkJsonConverter();
            yield return new HalLinksJsonConverter();
            yield return new HalEmbeddedJsonConverter();
            yield return new HalResourceJsonConverter();
        }
    }
}
