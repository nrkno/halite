using System;
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
            var json = JsonConvert.SerializeObject(link);
            json.ShouldBe("{\"templated\":true,\"href\":\"/user/{userId}\"}");
        }

        [Fact]
        public void VerifyBasicTemplatedLinkSerializationWithRelativeUrl()
        {
            var link = new HalTemplatedLink(new Uri("/user/{userId}", UriKind.Relative));
            var json = JsonConvert.SerializeObject(link);
            json.ShouldBe("{\"templated\":true,\"href\":\"/user/{userId}\"}");
        }

        [Fact]
        public void VerifyNamedTemplatedLinkSerialization()
        {
            var link = new HalTemplatedLink("/user/{userId}") { Name = "last" };
            var json = JsonConvert.SerializeObject(link);
            json.ShouldBe("{\"templated\":true,\"href\":\"/user/{userId}\",\"name\":\"last\"}");
        }

        [Fact]
        public void VerifyTypedTemplatedLinkSerialization()
        {
            var link = new HalTemplatedLink("/user/{userId}") { Type = "application/hal+json" };
            var json = JsonConvert.SerializeObject(link);
            json.ShouldBe("{\"templated\":true,\"href\":\"/user/{userId}\",\"type\":\"application/hal+json\"}");
        }
    }
}