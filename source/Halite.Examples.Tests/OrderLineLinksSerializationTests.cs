using Halite.Serialization.JsonNet;
using Newtonsoft.Json;
using Shouldly;
using Xunit;

namespace Halite.Examples.Tests
{
    public class OrderLineLinksSerializationTests
    {
        [Fact]
        public void VerifySerialization()
        {
            var links = new OrderLineLinks(new SelfLink("/orders/123"), new HalLink("/baskets/98712"),
                new HalLink("/customers/7809"));

            var json = Serialize(links);
            json.ShouldBe("{\"self\":{\"href\":\"/orders/123\"},\"ea:basket\":{\"href\":\"/baskets/98712\"},\"ea:customer\":{\"href\":\"/customers/7809\"}}");
        }

        private static string Serialize(object obj)
        {
            return JsonConvert.SerializeObject(obj, new HalLinkJsonConverter(), new HalLinksJsonConverter());
        }
    }
}