using System;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Halite
{
    /// <summary>
    /// Represents a HAL Link Object as described in the HAL specification.
    /// </summary>
    /// <remarks>
    /// Clients should never subclass this class, use HalLink or HalTemplatedLink instead.
    /// </remarks>
    [Serializable]
    public abstract class HalLinkObject
    {
        protected HalLinkObject([CanBeNull] Uri href)
        {
            Href = href ?? throw new ArgumentNullException(nameof(href));
        }

        protected HalLinkObject([CanBeNull] string href)
            : this(new Uri(href ?? throw new ArgumentNullException(nameof(href)), UriKind.RelativeOrAbsolute))
        {
        }

        /// <summary>
        /// 5.1. The "href" property is REQUIRED.
        /// 
        /// Its value is either a URI [RFC3986] or a URI Template [RFC6570].
        /// 
        /// If the value is a URI Template then the Link Object SHOULD have a
        /// "templated" attribute whose value is true.       
        /// </summary>
        [JsonProperty("href")]
        [NotNull]
        public Uri Href { get; }

        /// <summary>
        /// 5.2. The "templated" property is OPTIONAL.
        /// 
        /// Its value is boolean and SHOULD be true when the Link Object's "href"
        /// property is a URI Template.
        /// 
        /// Its value SHOULD be considered false if it is undefined. 
        /// </summary>
        [JsonProperty("templated", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [CanBeNull]
        public abstract bool? Templated { get; }

        /// <summary>
        /// 5.3. The "type" property is OPTIONAL.
        /// 
        /// Its value is a string used as a hint to indicate the media type
        /// expected when dereferencing the target resource.
        /// </summary>
        [JsonProperty("type", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [CanBeNull]
        public string Type { get; set; }

        /// <summary>
        /// 5.4. The "deprecation" property is OPTIONAL.
        /// 
        /// Its value is a string used as a hint to indicate the media type
        /// expected when dereferencing the target resource.
        /// </summary>
        [JsonProperty("deprecation", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [CanBeNull]
        public bool? Deprecation { get; set; }

        /// <summary>
        /// 5.5. The "name" property is OPTIONAL.
        /// 
        /// Its value MAY be used as a secondary key for selecting Link Objects
        /// which share the same relation type.
        /// </summary>
        [JsonProperty("name", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [CanBeNull]
        public string Name { get; set; }

        /// <summary>
        /// 5.6. The "profile" property is OPTIONAL.
        /// 
        /// Its value is a string which is URI that hints about the profile of the target resource.
        /// </summary>
        [JsonProperty("profile", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [CanBeNull]
        public Uri Profile { get; set; }

        /// <summary>
        /// 5.7. The "title" property is OPTIONAL.
        /// 
        /// Its value is a string and is intended for labelling the link with a human-readable identifier 
        /// (as defined by [RFC5988]).
        /// </summary>
        [JsonProperty("title", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [CanBeNull]
        public string Title { get; set; }

        /// <summary>
        /// 5.8. The "hreflang" property is OPTIONAL.
        /// 
        /// Its value is a string and is intended for indicating the language of the target resource 
        /// (as defined by [RFC5988]).
        /// </summary>
        [JsonProperty("hreflang", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [CanBeNull]
        public string HrefLang { get; set; }
    }
}
