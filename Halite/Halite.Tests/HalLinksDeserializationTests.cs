using Newtonsoft.Json;
using Shouldly;
using Xunit;

namespace Halite.Tests
{
    public class HalLinksDeserializationTests
    {
        [Fact]
        public void VerifyMinimalHalLinksDeserialization()
        {
            const string json = "{\"self\":{\"href\":\"/things/1\"}}";
            HalLinks links = JsonConvert.DeserializeObject<HalLinks>(json);
            var self = links.Self;
            self.Href.ToString().ShouldBe("/things/1");
            self.Templated.ShouldBeNull();
        }

        [Fact]
        public void VerifyDummyLinksDeserializationWithNulls()
        {
            const string json = "{\"self\":{\"href\":\"/things/1\"}}";
            DummyLinks links = JsonConvert.DeserializeObject<DummyLinks>(json);
            var selfLink = links.Self;
            selfLink.Href.ToString().ShouldBe("/things/1");
            selfLink.Templated.ShouldBeNull();
            links.This.ShouldBeNull();
            links.That.ShouldBeNull();
        }

        [Fact]
        public void VerifyDummyLinksDeserializationWithLinks()
        {
            const string json = "{\"self\":{\"href\":\"/things/1\"},\"this\":{\"href\":\"/this\"},\"that\":{\"href\":\"/that\"}}";
            DummyLinks links = JsonConvert.DeserializeObject<DummyLinks>(json);
            var selfLink = links.Self;
            selfLink.Href.ToString().ShouldBe("/things/1");
            selfLink.Templated.ShouldBeNull();
            links.This.Href.ToString().ShouldBe("/this");
            links.That.Href.ToString().ShouldBe("/that");
        }
    }
}