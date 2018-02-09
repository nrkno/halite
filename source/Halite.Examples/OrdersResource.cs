namespace Halite.Examples
{
    public class OrdersResource : HalResource<OrdersLinks, OrdersEmbedded>
    {
        [HalProperty("currentlyProcessing")]
        public int CurrentlyProcessing { get; set; }

        [HalProperty("shippedToday")]
        public int ShippedToday { get; set; }
    }
}
