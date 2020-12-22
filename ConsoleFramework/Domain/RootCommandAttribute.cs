
namespace ConsoleFramework.Domain
{
    public class RootCommandAttribute : CommandAttribute
    {
        public RootCommandAttribute(string? name = default)
        {
            Name = name;
        }

        public string? Name { get; }
    }
}