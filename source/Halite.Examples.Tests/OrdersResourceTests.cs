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

            var resource = new OrdersResource
            {
                Links = new OrdersLinks(curiesLinks, nextLink, findLink, adminLinks),
                CurrentlyProcessing = 14,
                ShippedToday = 20
            };

            var serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            var json = JsonConvert.SerializeObject(resource, Formatting.Indented, serializerSettings);
            json.ShouldBe(ReadJsonFile("OrdersResource1.json"));
        }
    }
}
