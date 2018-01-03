using Newtonsoft.Json;

namespace Halite.Tests
{
    internal class DummyLinks : HalLinks
    {
        public DummyLinks(SelfLink self, ThisLink @this, ThatLink that) : base(self)
        {
            This = @this;
            That = that;
        }

        [JsonProperty("this")]
        public ThisLink This { get; }

        [JsonProperty("that")]
        public ThatLink That { get; }
    }
}