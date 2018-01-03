using System.Linq;
using Newtonsoft.Json;
using Shouldly;
using Xunit;

namespace Halite.Tests
{
    public class HalResourceDeserializationTests
    {
        [Fact]
        public void VerifyDeserializeDummyResourceWithLinks()
        {
            const string json = "{\"_links\":{\"this\":{\"href\":\"/this\"},\"that\":{\"href\":\"/that\"},\"self\":{\"href\":\"/lambda\"}}}";
            DummyResourceWithLinks resource = JsonConvert.DeserializeObject<DummyResourceWithLinks>(json);
            resource.Links.Self.Href.ToString().ShouldBe("/lambda");
        }

        [Fact]
        public void VerifyDeserializeDummyResourceWithLinksAndNullEmbedded()
        {
            const string json = "{\"_links\":{\"this\":{\"href\":\"/this\"},\"that\":{\"href\":\"/that\"},\"self\":{\"href\":\"/lambda\"}}}";
            DummyResourceWithLinksAndEmbedded resource = JsonConvert.DeserializeObject<DummyResourceWithLinksAndEmbedded>(json);
            resource.Links.Self.Href.ToString().ShouldBe("/lambda");
            resource.Embedded.ShouldBeNull();
        }

        [Fact]
        public void VerifyDeserializeDummyResourceWithLinksAndEmbedded()
        {
            const string json = "{\"_links\":{\"this\":{\"href\":\"/this\"},\"that\":{\"href\":\"/that\"},\"self\":{\"href\":\"/lambda\"}},\"_embedded\":{}}";
            DummyResourceWithLinksAndEmbedded resource = JsonConvert.DeserializeObject<DummyResourceWithLinksAndEmbedded>(json);
            resource.Links.Self.Href.ToString().ShouldBe("/lambda");
            resource.Embedded.ShouldNotBeNull();
        }

        [Fact]
        public void VerifyDeserializeTurtleResource()
        {
            const string json =
                "{\"_links\":{\"self\":{\"href\":\"/turtle2\"}},\"_embedded\":{\"Down\":{\"_links\":{\"self\":{\"href\":\"/turtle1\"}},\"_embedded\":{\"Down\":{\"_links\":{\"self\":{\"href\":\"/turtle0\"}}}}}}}";
            TurtleResource turtle = JsonConvert.DeserializeObject<TurtleResource>(json);
            VerifyTurtles(turtle, "/turtle2", "/turtle1", "/turtle0");
        }

        private static void VerifyTurtles(TurtleResource turtle, params string[] expectedLinks)
        {
            foreach (var pair in turtle.AllTheWayDown().Select(t => t.Links.Self.Href.ToString())
                .Zip(expectedLinks, (actual, expected) => new { actual, expected }))
            {
                pair.actual.ShouldBe(pair.expected);
            }
        }
    }
}