namespace Halite.Examples
{
    public class OrderLineResource : HalResource<OrderLineLinks>
    {
        public OrderLineResource(decimal total, string currency, string status)
        {
            Total = total;
            Currency = currency;
            Status = status;
        }

        [HalProperty("total")]
        public decimal Total { get; }

        [HalProperty("currency")]
        public string Currency { get; }

        [HalProperty("status")]
        public string Status { get; }
    }
}