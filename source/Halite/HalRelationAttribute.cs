using System;

namespace Halite
{
    [AttributeUsage(AttributeTargets.Property)]
    public class HalRelationAttribute : Attribute
    {
        public HalRelationAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }
}