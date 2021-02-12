# Dotnet CommandLine framework
An experiment of trying to abstract away wiring of `dotnet/command-line-api` apps to build extensible commandline applications, inspired by setup of asp.net core. All classes listed below are registered in ServiceCollection and can have dependencies injected.

### Setup
````c#
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
````
### Service configuration
````c#
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<IService, Service>();
    }
}
````


### Rootcommand
````c#
[RootCommand]
public class TestRootCommand  : ConsoleCommand
{
    [Option("--namespace", Description = "Something")]
    public string Namespace { get; set; }

    public override int Invoke()
    {
        Console.WriteLine("here I Am");

        return 0;
    }
}

````

### Subcommand
````c#
[SubCommand("dotwo", typeof(TestRootCommand))]
class TestSubCommand2 : ConsoleCommand
{

    [Required]
    [Option("--namespace", "-n", Description = "Something", Suggester = typeof(NamespaceSuggester))]
    public string Namespace { get; set; }

    public override int Invoke()
    {
        Console.WriteLine("here I Am");

        return 0;
    }
}
````

### Suggester
```c#
internal class NamespaceSuggester : ISuggester
{
    private IService _service;
    
    public NamespaceSuggester(IService service)
    {
        _service = service;
    }

    public IEnumerable<string> GetSuggestions(ParseResult? parseResult = null, string? textToMatch = null)
    {
        return _service.GetSuggestions();
    }
}
```
