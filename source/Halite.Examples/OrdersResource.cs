namespace Halite.Examples
{
    public class OrdersResource : HalResource<OrdersLinks, OrdersEmbedded>
    {
        public int CurrentlyProcessing { get; set; }

        public int ShippedToday { get; set; }
    }
}
