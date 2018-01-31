using System.Collections.Generic;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Halite.Examples
{
    public class OrdersLinks : HalLinks
    {
        public OrdersLinks(IReadOnlyList<HalTemplatedLink> curiesLinks, HalLink nextLink, HalTemplatedLink findLink, IReadOnlyList<HalLink> adminLinks) : base(new SelfLink("/orders"))
        {
            CuriesLinks = curiesLinks;
            NextLink = nextLink;
            FindLink = findLink;
            AdminLinks = adminLinks;
        }

        [JsonProperty("curies", Order = 0)]
        [NotNull]
        public IReadOnlyList<HalTemplatedLink> CuriesLinks { get; }

        [JsonProperty("next", Order = 0)]
        [NotNull]
        public HalLink NextLink { get; }

        [JsonProperty("ea:find", Order = 0)]
        [NotNull]
        public HalTemplatedLink FindLink { get; }

        [JsonProperty("ea:admin", Order = 0)]
        [NotNull]
        public IReadOnlyList<HalLink> AdminLinks { get; }
    }
}