using System.CommandLine.Parsing;
using System.Threading.Tasks;
using ConsoleFramework;

namespace Playground
{
    public class Program
    {
        public static async Task<int> Main(string[] args)
        {
            var rootCommand = ConsoleApplicationBuilder
                .Create()
                .AddAllCommandsInEntryAssembly()
                .SetRootCommand<TestRootCommand>()
                .Startup<Startup>()
                .Build();

            return await rootCommand.InvokeAsync(args);
        }
    }
}