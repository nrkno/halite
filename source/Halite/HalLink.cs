using System;
using JetBrains.Annotations;

namespace Halite
{
    /// <summary>
    /// Represents a HAL link whose Href value is a URI [RFC3986]
    /// </summary>
    [Serializable]
    public class HalLink : HalLinkObject
    {
        public HalLink([CanBeNull] string href)
            : base(href)
        {
        }

        public HalLink([CanBeNull] Uri href) : base(href)
        {
        }

        /// <summary>
        /// This is not a templated link (you would use HalTemplatedLink for that). 
        /// Why do we return null instead of false? According to the HAL specification, 
        /// the value of the templated property SHOULD be considered false if it is undefined. 
        /// By choosing undefined over false, we avoid including the templated property 
        /// for non-templated links during serialization.
        /// </summary>
        public override bool? Templated => null;
    }
}