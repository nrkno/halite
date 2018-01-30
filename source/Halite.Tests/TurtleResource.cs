using System.Collections.Generic;

namespace Halite.Tests
{
    public class TurtleResource : HalResource<TurtleLinks, EmbeddedTurtle>
    {
        public IEnumerable<TurtleResource> AllTheWayDown()
        {
            yield return this;
            foreach (var t in Embedded?.Down?.AllTheWayDown() ?? new List<TurtleResource>())
            {
                yield return t;
            }
        }
    }
}