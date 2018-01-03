using JetBrains.Annotations;

namespace Halite
{
    public sealed class SelfLink : HalLink
    {
        public SelfLink([CanBeNull] string href) : base(href)
        {
        }
    }
}
