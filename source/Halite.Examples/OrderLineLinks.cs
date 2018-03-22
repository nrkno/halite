using JetBrains.Annotations;

namespace Halite.Examples
{
    public class OrderLineLinks : HalLinks
    {
        public OrderLineLinks([CanBeNull] SelfLink self, HalLink basketLink, HalLink customerLink) : base(self)
        {
            BasketLink = basketLink;
            CustomerLink = customerLink;
        }

        [HalRelation("ea:basket")]
        [NotNull]
        public HalLink BasketLink { get; }

        [HalRelation("ea:customer")]
        [NotNull]
        public HalLink CustomerLink { get; }
    }
}