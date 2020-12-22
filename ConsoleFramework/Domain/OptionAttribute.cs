using System;

namespace ConsoleFramework.Domain
{
    [AttributeUsage(AttributeTargets.Property)]
    public class OptionAttribute : Attribute
    {
        public string[] Aliases { get; }

        public OptionAttribute(params string[] aliases)
        {
            Aliases = aliases;
        }

        public string Description { get; set; } = "";

        public Type? Suggester { get; set; }
    }
}