using System;

namespace Halite
{
    [AttributeUsage(AttributeTargets.Property)]
    public class HalPropertyAttribute : Attribute
    {
        public HalPropertyAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }
}