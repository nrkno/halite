using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Shouldly;
using Xunit;

namespace Halite.Examples.Tests
{
    public class OrdersResourceTests
    {
        private static string ReadJsonFile(string fileName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var codeBaseUrl = new Uri(assembly.CodeBase);
            var codeBasePath = Uri.UnescapeDataString(codeBaseUrl.AbsolutePath);
            var dirPath = Path.GetDirectoryName(codeBasePath);
            var testDirPath = Path.Combine(dirPath, "TestFiles");
            var jsonFilePath = Path.Combine(testDirPath, fileName);
            return File.ReadAllText(jsonFilePath);
        } 

        [Fact]
        public void VerifyOrdersResource()
        {
            var resource = CreateOrdersResource();
            var json = Serialize(resource);
            json.ShouldBe(ReadJsonFile("OrdersResource.json"));
        }

        private static string Serialize(object o)
        {
            var serializer = new JsonSerializer
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Formatting = Formatting.Indented
            };

            using (var sw = new StringWriter())
            using (JsonWriter writer = new JsonTextWriter(sw) { Indentation = 2, IndentChar = ' ' })
            {
                serializer.Serialize(writer, o);
                return sw.ToString();
            }
        }

        private static OrdersResource CreateOrdersResource()
        {
            var curiesLinks = new List<HalTemplatedLink>
            {
                new HalTemplatedLink("http://example.com/docs/rels/{rel}")
                {
                    Name = "ea"
                }
            };
            var nextLink = new HalLink("/orders?page=2");
            var findLink = new HalTemplatedLink("/orders{?id}");
            var adminLinks = new List<HalLink>
            {
                new HalLink("/admins/2")
                {
                    Title = "Fred"
                },
                new HalLink("/admins/5")
                {
                    Title = "Kate"
                }
            };

            var orderLines = new List<OrderLineResource>
            {
                new OrderLineResource(30.00m, "USD", "shipped")
                {
                    Links = new OrderLineLinks(new SelfLink("/orders/123"), new HalLink("/baskets/98712"), new HalLink("/customers/7809"))
                },
                new OrderLineResource(20.00m, "USD", "processing")
                {
                    Links = new OrderLineLinks(new SelfLink("/orders/124"), new HalLink("/baskets/97213"), new HalLink("/customers/12369"))
                }
            };

            var resource = new OrdersResource
            {
                Links = new OrdersLinks(curiesLinks, nextLink, findLink, adminLinks),
                CurrentlyProcessing = 14,
                ShippedToday = 20,
                Embedded = new OrdersEmbedded(orderLines)
            };

            return resource;
        }
    }
}
