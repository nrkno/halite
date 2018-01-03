using System;

namespace Halite
{
    /// <summary>
    /// Superclass for embedded object for use in a HalResource.
    /// It is abstract because you will always want to subclass this to add some actual properties.
    /// Subclasses should obey the HAL specification:
    ///  - Property names are link relation types
    ///  - Values are either a resource object or an array of resource objects.
    /// This means property types must be either a HalResource subclass or
    /// (something that implements) IEnumerable of a HalResource subclass 
    /// </summary>
    [Serializable]
    public abstract class HalEmbedded
    {
    }
}