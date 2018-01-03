using JetBrains.Annotations;

namespace Halite.Tests
{
    public class TurtleLinks : HalLinks
    {
        public TurtleLinks([CanBeNull] SelfLink self) : base(self)
        {
        }
    }
}