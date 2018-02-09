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

        [HalRelation("curies")]
        [NotNull]
        public IReadOnlyList<HalTemplatedLink> CuriesLinks { get; }

        [HalRelation("next")]
        [NotNull]
        public HalLink NextLink { get; }

        [HalRelation("ea:find")]
        [NotNull]
        public HalTemplatedLink FindLink { get; }

        [HalRelation("ea:admin")]
        [NotNull]
        public IReadOnlyList<HalLink> AdminLinks { get; }
    }
}