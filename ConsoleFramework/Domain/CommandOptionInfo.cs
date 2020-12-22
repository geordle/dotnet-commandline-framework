using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using ConsoleFramework.Reflection;

namespace ConsoleFramework.Domain
{
    public class CommandOptionInfo
    {
        public PropertyInfo PropertyInfo { get; }

        private CommandOptionInfo(PropertyInfo propertyInfo)
        {
            PropertyInfo = propertyInfo;
            IsRequired = propertyInfo.HasAttribute<RequiredAttribute>();
            Attribute = propertyInfo
                .GetCustomAttributes(typeof(OptionAttribute), true)
                .Cast<OptionAttribute>()
                .FirstOrDefault() ?? throw new InvalidOperationException("Error attempting to construct CommandOptionInfo with non OptionAttribute annotated propery");
        }

        public OptionAttribute Attribute { get; }
        public bool IsRequired { get; }
        public static CommandOptionInfo Create(PropertyInfo arg)
        {
            return new(arg);
        }
    }
}