using System;
using Newtonsoft.Json;

namespace Halite
{
    [Serializable]
    public class HalResource<TLinks>
        where TLinks : HalLinks
    {
        [JsonProperty("_links", Order = -10)]
        public TLinks Links { get; set; }
    }

    [Serializable]
    public class HalResource<TLinks, TEmbedded> : HalResource<TLinks>
        where TLinks : HalLinks
        where TEmbedded : HalEmbedded
    {
        [JsonProperty("_embedded", Order = 100, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public TEmbedded Embedded { get; set; }
    }
}
