namespace Halite.Tests
{
    public class EmbeddedTurtle : HalEmbedded
    {
        [HalRelation("down")]
        public TurtleResource Down { get; set; }
    }
}