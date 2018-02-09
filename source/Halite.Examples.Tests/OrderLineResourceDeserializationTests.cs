using System.Collections.Generic;
using Halite.Serialization.JsonNet;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Shouldly;
using Xunit;

namespace Halite.Examples.Tests
{
    public class OrderLineResourceDeserializationTests
    {
        [Fact]
        public void VerifyDeserialization()
        {
            var json = "{\"_links\":{\"self\":{\"href\":\"/orders/123\"},\"ea:basket\":{\"href\":\"/baskets/98712\"},\"ea:customer\":{\"href\":\"/customers/7809\"}},\"total\":30.0,\"currency\":\"USD\",\"status\":\"shipped\"}";
            var resource = Deserialize<OrderLineResource>(json);
            resource.Links.ShouldNotBeNull();
            resource.Links.Self.Href.ToString().ShouldBe("/orders/123");
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
                    new HalResourceJsonConverter()
                }
            };
            return JsonConvert.DeserializeObject<T>(json, settings);
        }
    }
}