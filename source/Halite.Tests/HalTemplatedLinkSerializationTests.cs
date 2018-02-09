using System;
using Halite.Serialization.JsonNet;
using Newtonsoft.Json;
using Shouldly;
using Xunit;

namespace Halite.Tests
{
    public class HalTemplatedLinkSerializationTests
    {
        [Fact]
        public void VerifyBasicTemplatedLinkSerialization()
        {
            var link = new HalTemplatedLink("/user/{userId}");
            var json = Serialize(link);
            json.ShouldBe("{\"href\":\"/user/{userId}\",\"templated\":true}");
        }

        [Fact]
        public void VerifyBasicTemplatedLinkSerializationWithRelativeUrl()
        {
            var link = new HalTemplatedLink(new Uri("/user/{userId}", UriKind.Relative));
            var json = Serialize(link);
            json.ShouldBe("{\"href\":\"/user/{userId}\",\"templated\":true}");
        }

        [Fact]
        public void VerifyNamedTemplatedLinkSerialization()
        {
            var link = new HalTemplatedLink("/user/{userId}") { Name = "last" };
            var json = Serialize(link);
            json.ShouldBe("{\"name\":\"last\",\"href\":\"/user/{userId}\",\"templated\":true}");
        }

        [Fact]
        public void VerifyTypedTemplatedLinkSerialization()
        {
            var link = new HalTemplatedLink("/user/{userId}") { Type = "application/hal+json" };
            var json = Serialize(link);
            json.ShouldBe("{\"href\":\"/user/{userId}\",\"templated\":true,\"type\":\"application/hal+json\"}");
        }

        private static string Serialize<T>(T link) where T : HalTemplatedLink
        {
            return JsonConvert.SerializeObject(link, new HalLinkJsonConverter());
        }
    }
}