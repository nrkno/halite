using System.Collections.Generic;
using Newtonsoft.Json;

namespace Halite.Examples
{
    public class OrdersEmbedded : HalEmbedded
    {
        public OrdersEmbedded(IReadOnlyList<OrderLineResource> orderLines)
        {
            OrderLines = orderLines;
        }

        [JsonProperty("ea:order")]
        public IReadOnlyList<OrderLineResource> OrderLines { get; }
    }
}