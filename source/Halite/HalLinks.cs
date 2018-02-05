using System;
using JetBrains.Annotations;

namespace Halite
{
    /// <summary>
    /// Basic collection of HAL links belonging to a resource.
    /// You might want to subclass this for a specific resource 
    /// and add link properties as appropriate.
    /// </summary>
    public class HalLinks
    {
        public HalLinks([CanBeNull] SelfLink self)
        {
            Self = self ?? throw new ArgumentNullException(nameof(self));
        }

        /// <summary>
        /// Mandatory 'self' link for all resources.
        /// </summary>
        [HalProperty("self")]
        [NotNull]
        public SelfLink Self { get; }
    }
}