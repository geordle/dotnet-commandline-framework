using System;
using System.Reflection;

namespace ConsoleFramework.Reflection
{
    public static class TypeExtensions
    {
        public static bool HasAttribute<TAttribute>(this PropertyInfo type) where TAttribute : Attribute
        {
            return type.GetCustomAttribute<TAttribute>() is { };
        }
    }
}