using System;

namespace ConsoleFramework.Domain
{
    public class SubCommandAttribute : CommandAttribute
    {
        public SubCommandAttribute(string name, Type parent)
        {
            Name = name;
            Parent = parent;
        }

        public string Name { get; }

        public Type Parent { get; }
    }
}