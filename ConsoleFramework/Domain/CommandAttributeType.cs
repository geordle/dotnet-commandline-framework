using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ConsoleFramework.Domain
{
    public class CommandAttributeType<TAttribute> where TAttribute : CommandAttribute
    {

        private CommandAttributeType(Type type, TAttribute attribute)
        {
            Type = type;
            Attribute = attribute;

            Properties = Type.GetProperties().Select(CommandOptionInfo.Create).ToList();
        }

        public List<CommandOptionInfo> Properties { get; }

        public static CommandAttributeType<TAttribute>? Create(Type arg)
        {
            var commandAttribute = arg.GetCustomAttribute<TAttribute>();
            if (commandAttribute is null)
            {
                return null;
            }
            return new CommandAttributeType<TAttribute>(arg, commandAttribute);
        }

        public TAttribute Attribute { get; }

        public Type Type { get; }

    }
}