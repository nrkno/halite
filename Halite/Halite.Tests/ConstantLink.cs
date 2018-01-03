namespace Halite.Tests
{
    public class ConstantLink : HalLink
    {
        public const string AlwaysTheSame = "/always/the/same";

        public ConstantLink() : base(AlwaysTheSame)
        {
        }
    }
}
