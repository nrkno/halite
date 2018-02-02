using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Halite.Examples
{
    public class OrderLineLinks : HalLinks
    {
        public OrderLineLinks([CanBeNull] SelfLink self, HalLink basketLink, HalLink customerLink) : base(self)
        {
            BasketLink = basketLink;
            CustomerLink = customerLink;
        }

        [JsonProperty("ea:basket", Order = 0)]
        [NotNull]
        public HalLink BasketLink { get; }

        [JsonProperty("ea:customer", Order = 0)]
        [NotNull]
        public HalLink CustomerLink { get; }
    }
}