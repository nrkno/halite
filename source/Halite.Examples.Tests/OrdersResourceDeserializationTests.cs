using System.Collections.Generic;
using Halite.Serialization.JsonNet;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Shouldly;
using Xunit;

namespace Halite.Examples.Tests
{
    public class OrdersResourceDeserializationTests
    {
        [Fact]
        public void VerifyOrdersResourceDeserialization()
        {
            var json = JsonTestFile.Read("OrdersResource.json");
            var resource = Deserialize<OrdersResource>(json);
            resource.ShouldNotBeNull();
        }

        private static T Deserialize<T>(string json)
        {
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Converters = new List<JsonConverter>()
                {
                    new HalLinkJsonConverter(),
                    new HalLinksJsonConverter(),
                    new HalEmbeddedJsonConverter(),
                    new HalResourceJsonConverter()
                }
            };
            return JsonConvert.DeserializeObject<T>(json, settings);
        }
    }
}