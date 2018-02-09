using System;
using Halite.Serialization.JsonNet;
using Newtonsoft.Json;
using Shouldly;
using Xunit;

namespace Halite.Tests
{
    public class HalLinkSerializationTests
    {
        [Fact]
        public void VerifyBasicLinkSerialization()
        {
            var link = new HalLink("/user/{userId}");
            var json = Serialize(link);
            json.ShouldBe("{\"href\":\"/user/{userId}\"}");
        }

        [Fact]
        public void VerifyBasicLinkSerializationWithRelativeUrl()
        {
            var link = new HalLink(new Uri("/user/{userId}", UriKind.Relative));
            var json = Serialize(link);
            json.ShouldBe("{\"href\":\"/user/{userId}\"}");
        }

        [Fact]
        public void VerifyNamedLinkSerialization()
        {
            var link = new HalLink("/user/{userId}") { Name = "last" };
            var json = Serialize(link);
            json.ShouldBe("{\"name\":\"last\",\"href\":\"/user/{userId}\"}");
        }

        [Fact]
        public void VerifyTypedLinkSerialization()
        {
            var link = new HalLink("/user/{userId}") { Type = "application/hal+json" };
            var json = Serialize(link);
            json.ShouldBe("{\"href\":\"/user/{userId}\",\"type\":\"application/hal+json\"}");
        }

        private static string Serialize<T>(T link) where T : HalLink
        {
            return JsonConvert.SerializeObject(link, new HalLinkJsonConverter());
        }
    }
}