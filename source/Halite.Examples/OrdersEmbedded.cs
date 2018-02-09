using System.Collections.Generic;

namespace Halite.Examples
{
    public class OrdersEmbedded : HalEmbedded
    {
        public OrdersEmbedded(IReadOnlyList<OrderLineResource> orderLines)
        {
            OrderLines = orderLines;
        }

        [HalRelation("ea:order")]
        public IReadOnlyList<OrderLineResource> OrderLines { get; }
    }
}