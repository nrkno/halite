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

        public decimal Total { get; }

        public string Currency { get; }

        public string Status { get; }
    }
}