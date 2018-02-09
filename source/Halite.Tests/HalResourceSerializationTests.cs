using System.Collections.Generic;
using Halite.Serialization.JsonNet;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
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
                Links = new DummyLinks(new SelfLink("/lambda"),
                    new ThisLink(), 
                    new ThatLink(), 
                    new List<HalLink>
                    {
                        new HalLink("/quux"),
                        new HalLink("/xuuq")
                    })
            };

            var json = Serialize(resource);
            json.ShouldBe("{\"_links\":{\"self\":{\"href\":\"/lambda\"},\"this\":{\"href\":\"/this\"},\"that\":{\"href\":\"/that\"},\"those\":[{\"href\":\"/quux\"},{\"href\":\"/xuuq\"}]}}");
        }

        [Fact]
        public void VerifyResourceWithLinksAndEmbedded()
        {
            var resource = new DummyResourceWithLinksAndEmbedded
            {
                Links = new DummyLinks(new SelfLink("/lambda"), 
                new ThisLink(), 
                new ThatLink(),
                new List<HalLink>
                {
                    new HalLink("/quux"),
                    new HalLink("/xuuq")
                }),
                Embedded = new DummyEmbedded()
            };

            var json = Serialize(resource);
            json.ShouldBe("{\"_links\":{\"self\":{\"href\":\"/lambda\"},\"this\":{\"href\":\"/this\"},\"that\":{\"href\":\"/that\"},\"those\":[{\"href\":\"/quux\"},{\"href\":\"/xuuq\"}]},\"_embedded\":{}}");
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

            var json = Serialize(turtle);
            var expectedJson =
                "{\"_links\":{\"self\":{\"href\":\"/turtle2\"}},\"_embedded\":{\"down\":{\"_links\":{\"self\":{\"href\":\"/turtle1\"}},\"_embedded\":{\"down\":{\"_links\":{\"self\":{\"href\":\"/turtle0\"}}}}}}}";
            json.ShouldBe(expectedJson);
        }

        private static string Serialize(object obj)
        {
            return JsonConvert.SerializeObject(obj, new JsonSerializerSettings().ConfigureForHalite());
        }
    }
}