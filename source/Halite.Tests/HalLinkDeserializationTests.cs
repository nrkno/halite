using System;
using Halite.Serialization.JsonNet;
using Newtonsoft.Json;
using Shouldly;
using Xunit;

namespace Halite.Tests
{
    public class HalLinkDeserializationTests
    {
        [Fact]
        public void VerifyBasicDeserializationToHalLink()
        {
            const string json = "{\"href\":\"/things/1\"}";
            HalLink link = Deserialize<HalLink>(json);
            link.Href.ToString().ShouldBe("/things/1");
            link.Templated.ShouldBeNull();
        }

        [Fact]
        public void VerifyBasicDeserializationToHalTemplatedLink()
        {
            const string json = "{\"href\":\"/things/1\"}";
            HalTemplatedLink link = Deserialize<HalTemplatedLink>(json);
            link.Href.ToString().ShouldBe("/things/1");
            link.Templated.ShouldBe(true);
        }

        [Fact]
        public void VerifyBasicDeserializationToSelfLink()
        {
            const string json = "{\"href\":\"/things/1\"}";
            var link = Deserialize<SelfLink>(json);
            link.Href.ToString().ShouldBe("/things/1");
        }

        [Fact]
        public void VerifyDeserializationOfConstantLink()
        {
            const string json = "{\"href\":\"/a/different/link\"}";
            var link = Deserialize<ConstantLink>(json);
            link.Href.ToString().ShouldNotBe("/a/different/link");
            link.Href.ToString().ShouldBe("/always/the/same");
        }

        [Fact]
        public void VerifyNamedLinkDeserialization()
        {
            const string json = "{\"href\":\"/things/1\",\"name\":\"first\"}";
            HalLink link = Deserialize<HalLink>(json);
            link.Href.ToString().ShouldBe("/things/1");
            link.Name.ShouldBe("first");
        }

        [Fact]
        public void VerifyTypedLinkDeserialization()
        {
            const string json = "{\"href\":\"/things/1\",\"type\":\"application/hal+json\"}";
            HalLink link = Deserialize<HalLink>(json);
            link.Href.ToString().ShouldBe("/things/1");
            link.Type.ShouldBe("application/hal+json");
        }

        [Fact]
        public void VerifyTypedSelfLinkDeserialization()
        {
            const string json = "{\"href\":\"/things/1\",\"type\":\"application/hal+json\"}";
            SelfLink link = Deserialize<SelfLink>(json);
            link.Href.ToString().ShouldBe("/things/1");
            link.Type.ShouldBe("application/hal+json");
        }

        [Fact]
        public void VerifyAmbitiousLinkDeserialization()
        {
            var json =
                "{\"href\":\"/things/1\",\"type\":\"application/hal+json\",\"deprecation\":true,\"name\":\"quux\",\"profile\":\"http://some/profile\",\"title\":\"Link to something\",\"hreflang\":\"en\"}";
            HalLink link = Deserialize<HalLink>(json);
            link.Href.ToString().ShouldBe("/things/1");
            link.Type.ShouldBe("application/hal+json");
            link.Deprecation.ShouldBe(true);
            link.Name.ShouldBe("quux");
            link.Profile.ToString().ShouldBe("http://some/profile");
            link.Title.ShouldBe("Link to something");
            link.HrefLang.ShouldBe("en");
        }

        [Fact]
        public void VerifyCantDeserializeEmptyJsonObject()
        {
            const string json = "{}";
            Assert.Throws<JsonSerializationException>(() => Deserialize<HalLink>(json));
        }

        [Fact]
        public void VerifyCantDeserializeEmptyJsonObject1()
        {
            const string json = "{}";
            Assert.Throws<JsonSerializationException>(() => JsonConvert.DeserializeObject<HalLink>(json));
        }


        [Fact]
        public void VerifyCantDeserializeWithoutHref()
        {
            const string json = "{\"name\":\"quux\"}";
            Assert.Throws<JsonSerializationException>(() => Deserialize<HalLink>(json));
        }

        [Fact]
        public void VerifyUnknownPropertiesAreIgnored()
        {
            const string json = "{\"href\":\"/things/1\",\"baa\":\"moo\"}";
            var link = Deserialize<HalLink>(json);
            link.Href.ToString().ShouldBe("/things/1");
        }

        [Fact]
        public void VerifyHrefCannotBeNumber()
        {
            const string json = "{\"href\":0}";
            Assert.Throws<JsonSerializationException>(() => Deserialize<HalLink>(json));
        }

        [Fact]
        public void VerifyHrefCannotBeBoolean()
        {
            const string json = "{\"href\":true}";
            Assert.Throws<JsonSerializationException>(() => Deserialize<HalLink>(json));
        }

        private static T Deserialize<T>(string json) where T : HalLinkObject
        {
            return JsonConvert.DeserializeObject<T>(json, new HalLinkJsonConverter());
        }
    }
}