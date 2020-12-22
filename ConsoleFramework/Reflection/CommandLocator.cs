using System;
using System.Linq;
using System.Reflection;
using ConsoleFramework.Domain;

namespace ConsoleFramework.Reflection
{
    public static class CommandLocator
    {
        public static CommandAttributeType<RootCommandAttribute> GetRootCommand(Assembly commandAssembly)
        {
            var typesWithAttribute = TypeLocator.GetTypesWithAttribute<RootCommandAttribute>(commandAssembly).ToList();
            if (typesWithAttribute.Count > 1)
            {
                throw new Exception("more than one Command with RootCommand Attribute found in assembly.");
            }

            if (!typesWithAttribute.Any())
            {
                throw new Exception("No command marked with RootCommand Attribute found in assembly");
            }

            return typesWithAttribute.First();
        }
    }
}