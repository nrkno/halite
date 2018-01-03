using Newtonsoft.Json;
using Shouldly;
using Xunit;

namespace Halite.Tests
{
    public class HalResourceSerializationTests
    {
        [Fact]
        public void VerifyResourceWithLinks()
        {
            var resource = new DummyResourceWithLinks
            {
                Links = new DummyLinks(new SelfLink("/lambda"), new ThisLink(), new ThatLink())
            };

            var json = JsonConvert.SerializeObject(resource);
            json.ShouldBe("{\"_links\":{\"this\":{\"href\":\"/this\"},\"that\":{\"href\":\"/that\"},\"self\":{\"href\":\"/lambda\"}}}");
        }

        [Fact]
        public void VerifyResourceWithLinksAndEmbedded()
        {
            var resource = new DummyResourceWithLinksAndEmbedded
            {
                Links = new DummyLinks(new SelfLink("/lambda"), new ThisLink(), new ThatLink()),
                Embedded = new DummyEmbedded()
            };

            var json = JsonConvert.SerializeObject(resource);
            json.ShouldBe("{\"_links\":{\"this\":{\"href\":\"/this\"},\"that\":{\"href\":\"/that\"},\"self\":{\"href\":\"/lambda\"}},\"_embedded\":{}}");
        }

        [Fact]
        public void VerifyTurtleResourceSerialization()
        {
            var turtle = new TurtleResource
            {
                Links = new TurtleLinks(new SelfLink("/turtle2")),
                Embedded = new EmbeddedTurtle
                {
                    Down = new TurtleResource
                    {
                        Links = new TurtleLinks(new SelfLink("/turtle1")),
                        Embedded = new EmbeddedTurtle
                        {
                            Down = new TurtleResource { Links = new TurtleLinks(new SelfLink("/turtle0")) }
                        }
                    }
                }
            };

            var json = JsonConvert.SerializeObject(turtle);
            var expectedJson =
                "{\"_links\":{\"self\":{\"href\":\"/turtle2\"}},\"_embedded\":{\"Down\":{\"_links\":{\"self\":{\"href\":\"/turtle1\"}},\"_embedded\":{\"Down\":{\"_links\":{\"self\":{\"href\":\"/turtle0\"}}}}}}}";
            json.ShouldBe(expectedJson);
        }
    }
}