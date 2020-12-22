using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ConsoleFramework.Domain;

namespace ConsoleFramework.Reflection
{
    public static class TypeLocator
    {
        public static IEnumerable<CommandAttributeType<TAttribute>> GetTypesWithAttribute<TAttribute>(Assembly target)
            where TAttribute : CommandAttribute
        {
            return target
                .GetTypes()
                .Select(CommandAttributeType<TAttribute>.Create)
                .Where(arg => arg is { })!;
        }
    }
}