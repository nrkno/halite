using System.Collections.Generic;
using System.IO;
using Halite.Serialization.JsonNet;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Shouldly;
using Xunit;

namespace Halite.Examples.Tests
{
    public class OrdersEmbeddedSerializationTests
    {
        [Fact]
        public void VerifySerialization()
        {
            var embedded = CreateOrdersEmbedded();
            var json = Serialize(embedded);
            json.ShouldNotBeNull();
            json.ShouldBe(JsonTestFile.Read("OrdersEmbedded.json"));
        }

        private static OrdersEmbedded CreateOrdersEmbedded()
        {
            return new OrdersEmbedded(new List<OrderLineResource>
            {
                new OrderLineResource(30.00m, "USD", "shipped")
                {
                    Links = new OrderLineLinks(new SelfLink("/orders/123"),
                        new HalLink("/baskets/98712"),
                        new HalLink("/customers/7809"))
                },
                new OrderLineResource(20.00m, "USD", "processing")
                {
                    Links = new OrderLineLinks(new SelfLink("/orders/124"),
                        new HalLink("/baskets/97213"),
                        new HalLink("/customers/12369"))
                }
            });
        }

        private static string Serialize(object o)
        {
            var serializer = new JsonSerializer
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Formatting = Formatting.Indented,
                Converters =
                {
                    new HalLinkJsonConverter(),
                    new HalLinksJsonConverter(),
                    new HalEmbeddedJsonConverter(),
                    new HalResourceJsonConverter()
                }
            };

            using (var sw = new StringWriter { NewLine = "\n" })
            using (JsonWriter writer = new JsonTextWriter(sw)
            {
                Indentation = 2,
            })
            {
                serializer.Serialize(writer, o);
                return sw.ToString();
            }
        }
    }
}