using System;
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
            HalLink link = JsonConvert.DeserializeObject<HalLink>(json);
            link.Href.ToString().ShouldBe("/things/1");
            link.Templated.ShouldBeNull();
        }

        [Fact]
        public void VerifyBasicDeserializationToHalTemplatedLink()
        {
            const string json = "{\"href\":\"/things/1\"}";
            HalTemplatedLink link = JsonConvert.DeserializeObject<HalTemplatedLink>(json);
            link.Href.ToString().ShouldBe("/things/1");
            link.Templated.ShouldBe(true);
        }

        [Fact]
        public void VerifyBasicDeserializationToSelfLink()
        {
            const string json = "{\"href\":\"/things/1\"}";
            var link = JsonConvert.DeserializeObject<SelfLink>(json);
            link.Href.ToString().ShouldBe("/things/1");
        }

        [Fact]
        public void VerifyDeserializationOfConstantLink()
        {
            const string json = "{\"href\":\"/a/different/link\"}";
            var link = JsonConvert.DeserializeObject<ConstantLink>(json);
            link.Href.ToString().ShouldNotBe("/a/different/link");
            link.Href.ToString().ShouldBe("/always/the/same");
        }

        [Fact]
        public void VerifyNamedLinkDeserialization()
        {
            const string json = "{\"href\":\"/things/1\",\"name\":\"first\"}";
            HalLink link = JsonConvert.DeserializeObject<HalLink>(json);
            link.Href.ToString().ShouldBe("/things/1");
            link.Name.ShouldBe("first");
        }

        [Fact]
        public void VerifyTypedLinkDeserialization()
        {
            const string json = "{\"href\":\"/things/1\",\"type\":\"application/hal+json\"}";
            HalLink link = JsonConvert.DeserializeObject<HalLink>(json);
            link.Href.ToString().ShouldBe("/things/1");
            link.Type.ShouldBe("application/hal+json");
        }

        [Fact]
        public void VerifyTypedSelfLinkDeserialization()
        {
            const string json = "{\"href\":\"/things/1\",\"type\":\"application/hal+json\"}";
            SelfLink link = JsonConvert.DeserializeObject<SelfLink>(json);
            link.Href.ToString().ShouldBe("/things/1");
            link.Type.ShouldBe("application/hal+json");
        }

        [Fact]
        public void VerifyAmbitiousLinkDeserialization()
        {
            var json =
                "{\"href\":\"/things/1\",\"type\":\"application/hal+json\",\"deprecation\":true,\"name\":\"quux\",\"profile\":\"http://some/profile\",\"title\":\"Link to something\",\"hreflang\":\"en\"}";
            HalLink link = JsonConvert.DeserializeObject<HalLink>(json);
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
            Assert.Throws<ArgumentNullException>(() => JsonConvert.DeserializeObject<HalLink>(json));
        }

        [Fact]
        public void VerifyCantDeserializeWithoutHref()
        {
            const string json = "{\"name\":\"quux\"}";
            Assert.Throws<ArgumentNullException>(() => JsonConvert.DeserializeObject<HalLink>(json));
        }

        [Fact]
        public void VerifyUnknownPropertiesAreIgnored()
        {
            const string json = "{\"href\":\"/things/1\",\"baa\":\"moo\"}";
            var link = JsonConvert.DeserializeObject<HalLink>(json);
            link.Href.ToString().ShouldBe("/things/1");
        }

        [Fact]
        public void VerifyHrefCanBeNumberButItsWeird()
        {
            const string json = "{\"href\":0}";
            var link = JsonConvert.DeserializeObject<HalLink>(json);
            link.Href.ToString().ShouldBe("0");
        }

        [Fact]
        public void VerifyHrefCanBeBooleanButItsWeird()
        {
            const string json = "{\"href\":true}";
            var link = JsonConvert.DeserializeObject<HalLink>(json);
            link.Href.ToString().ShouldBe("True");
        }
    }
}