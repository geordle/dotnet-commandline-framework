using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.Linq;
using System.Threading;
using ConsoleFramework.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace ConsoleFramework
{
    internal class CommandFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public CommandFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Command CreateCommand(CommandAttributeType<SubCommandAttribute> commandInfo)
        {
            var command = new Command(commandInfo.Attribute.Name);
            foreach (var property in commandInfo.Properties)
            {
                var option = ToOption(property);
                command.AddOption(option);
                if (property.Attribute.Suggester is {})
                {
                    var suggestionSource = (ISuggester)_serviceProvider.GetRequiredService(property.Attribute.Suggester);
                    option.AddSuggestions((parseResult, match) => suggestionSource.GetSuggestions(parseResult, match));
                }
            }

            command.Handler = GetCommandHandler(commandInfo);
            return command;
        }

        private ICommandHandler GetCommandHandler(CommandAttributeType<SubCommandAttribute> commandInfo)
        {
            return CommandHandler.Create((ParseResult parseResult, CancellationToken cancellationToken) =>
            {
                var cancellationTokenAccessor = _serviceProvider.GetRequiredService<CancellationTokenAccessor>();
                cancellationTokenAccessor.SetCancellationToken(cancellationToken);
                var requiredService = (ConsoleCommand) _serviceProvider.GetRequiredService(commandInfo.Type);
                BindArgumentsToProperties(commandInfo, parseResult, requiredService);
                return requiredService.Invoke();
            });
        }

        private static Option ToOption(CommandOptionInfo info)
        {
            return new(info.Attribute.Aliases)
            {
                Description = info.Attribute.Description,
                IsRequired = info.IsRequired,
                Argument = new Argument {ArgumentType = info.PropertyInfo.PropertyType}
            };
        }

        private static void BindArgumentsToProperties(CommandAttributeType<SubCommandAttribute> commandInfo,
            ParseResult parseResult,
            ConsoleCommand requiredService)
        {
            foreach (var rootCommandProperty in commandInfo.Properties)
            {
                var valueForOption = parseResult.ValueForOption(rootCommandProperty.Attribute.Aliases.First());
                rootCommandProperty.PropertyInfo.SetValue(requiredService, valueForOption);
            }
        }
    }
}