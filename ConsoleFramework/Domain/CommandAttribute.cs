using System;

namespace ConsoleFramework.Domain
{
    [AttributeUsage(AttributeTargets.Class)]
    public abstract class CommandAttribute : Attribute
    {
    }
}