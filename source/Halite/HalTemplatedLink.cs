using System;
using JetBrains.Annotations;

namespace Halite
{
    /// <summary>
    /// Represents a HAL link whose Href value is a URI Template [RFC6570].
    /// </summary>
    [Serializable]
    public class HalTemplatedLink : HalLinkObject
    {
        public HalTemplatedLink([CanBeNull] string href) : base(href)
        {
        }

        public HalTemplatedLink([CanBeNull] Uri href) : base(href)
        {
        }

        /// <summary>
        /// Templated link is templated.
        /// </summary>
        public override bool? Templated => true;
    }
}