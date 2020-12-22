using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using System.Linq;
using System.Reflection;
using ConsoleFramework.Domain;
using ConsoleFramework.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace ConsoleFramework
{
    public class ConsoleApplicationBuilder
    {
        private readonly HashSet<CommandAttributeType<SubCommandAttribute>> _commandAttributeTypes = new();
        private CommandAttributeType<RootCommandAttribute>? _rootCommandAttibute;

        private ConsoleApplicationBuilder()
        {
            ApplicationServiceCollection = new ServiceCollection();
        }

        private ServiceCollection ApplicationServiceCollection { get; set; }

        public static ConsoleApplicationBuilder Create()
        {
            return new();
        }

        public Parser Build()
        {
            foreach (var commandAttributeType in _commandAttributeTypes)
            {
                ApplicationServiceCollection.AddSingleton(commandAttributeType.Type);
            }

            ApplicationServiceCollection.AddSingleton<CancellationTokenAccessor>();
            var rootCommandAttribute = _rootCommandAttibute ?? CommandLocator.GetRootCommand(
                Assembly.GetEntryAssembly() ??
                throw new InvalidOperationException(
                    "No RootCommand set and no executing assembly availabe"));
            ApplicationServiceCollection.AddSingleton(rootCommandAttribute.Type);
            RegisterCommandTypes();


            var serviceProvider = ApplicationServiceCollection.BuildServiceProvider();
            var rootCommand = BuildCommandTree(serviceProvider, rootCommandAttribute);


            var parser = new CommandLineBuilder(rootCommand)
                .UseVersionOption()
                .UseHelp()
                .UseParseDirective()
                .UseDebugDirective()
                .UseSuggestDirective()
                .RegisterWithDotnetSuggest()
                .UseTypoCorrections()
                .UseParseErrorReporting()
                .CancelOnProcessTermination()
                .Build();
            return parser;
        }

        private void RegisterCommandTypes()
        {
            var types = _commandAttributeTypes
                .SelectMany(type =>
                    type.Properties.Select(info => info.Attribute.Suggester).Append(type.Type))
                .Where(type => type is { });
            foreach (var type in types)
            {
                ApplicationServiceCollection.AddSingleton(type!);
            }
        }

        private RootCommand BuildCommandTree(IServiceProvider serviceProvider,
            CommandAttributeType<RootCommandAttribute> rootCommandAttribute)
        {
            var commandFactory = new CommandFactory(serviceProvider);

            var commands = _commandAttributeTypes
                .Select(t => new {Type = t, Command = commandFactory.CreateCommand(t)}).ToList();

            var subCommandsGroupedByParent = commands
                .GroupBy(arg => arg.Type.Attribute.Parent);

            var parentCommandMap = commands.ToDictionary(arg => arg.Type.Type, arg => arg.Command);

            RootCommand rootCommand = rootCommandAttribute.Attribute.Name != null
                ? new RootCommand(rootCommandAttribute.Attribute.Name)
                : new RootCommand();

            parentCommandMap[rootCommandAttribute.Type] = rootCommand;
            foreach (var subCommands in subCommandsGroupedByParent)
            {
                var parent = parentCommandMap[subCommands.Key];
                foreach (var subCommand in subCommands)
                {
                    parent.AddCommand(subCommand.Command);
                }
            }

            return rootCommand;
        }

        public ConsoleApplicationBuilder Startup<T>() where T : class, new()
        {
            var foo = new T();
            foo.GetType().GetMethod("ConfigureServices")?.Invoke(foo, new object?[] {ApplicationServiceCollection});
            return this;
        }

        public ConsoleApplicationBuilder Startup(Action<IServiceCollection> configurator)
        {
            configurator(ApplicationServiceCollection);
            return this;
        }

        public ConsoleApplicationBuilder AddAllCommandsInEntryAssembly()
        {
            AddAllCommandsInAssembly(Assembly.GetEntryAssembly() ??
                                     throw new InvalidOperationException(
                                         "Unable register commands, no entry assembly present."));
            return this;
        }

        public ConsoleApplicationBuilder AddAllCommandsInAssembly(Assembly getAssembly)
        {
            var commandAttributeTypes = TypeLocator.GetTypesWithAttribute<SubCommandAttribute>(getAssembly);
            _commandAttributeTypes.UnionWith(commandAttributeTypes);
            return this;
        }

        public ConsoleApplicationBuilder SetRootCommand<T>()
        {
            var rootCommandAttribute = CommandAttributeType<RootCommandAttribute>.Create(typeof(T));
            _rootCommandAttibute = rootCommandAttribute ??
                                   throw new InvalidOperationException(
                                       "Type entered is does not have RootCommandAttribute set.");
            return this;
        }
    }
}