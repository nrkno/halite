using System;

namespace Halite
{
    
    public class HalResource<TLinks>
        where TLinks : HalLinks
    {
        [HalProperty("_links")]
        public TLinks Links { get; set; }
    }

    
    public class HalResource<TLinks, TEmbedded> : HalResource<TLinks>
        where TLinks : HalLinks
        where TEmbedded : HalEmbedded
    {
        [HalProperty("_embedded")]
        public TEmbedded Embedded { get; set; }
    }
}
