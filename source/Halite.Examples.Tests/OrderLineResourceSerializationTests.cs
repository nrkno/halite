using System.Collections.Generic;
using Halite.Serialization.JsonNet;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Shouldly;
using Xunit;

namespace Halite.Examples.Tests
{
    public class OrderLineResourceSerializationTests
    {
        [Fact]
        public void VerifySerialization()
        {
            var resource = new OrderLineResource(new decimal(30.00), "USD", "shipped")
            {
                Links = new OrderLineLinks(new SelfLink("/orders/123"),
                    new HalLink("/baskets/98712"),
                    new HalLink("/customers/7809"))
            };

            var json = Serialize(resource);
            json.ShouldBe("{\"_links\":{\"self\":{\"href\":\"/orders/123\"},\"ea:basket\":{\"href\":\"/baskets/98712\"},\"ea:customer\":{\"href\":\"/customers/7809\"}},\"total\":30.0,\"currency\":\"USD\",\"status\":\"shipped\"}");
        }

        private static string Serialize(object obj)
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
            return JsonConvert.SerializeObject(obj, settings);
        }
    }
}