using System;
using System.Collections.Generic;
using System.CommandLine.Parsing;
using System.ComponentModel.DataAnnotations;
using ConsoleFramework;
using ConsoleFramework.Domain;

namespace Playground
{

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


    [SubCommand("do", typeof(TestRootCommand))]
    class TestSubCommand : ConsoleCommand
    {

        [Option("--namespace", Description = "Something")]
        public string Namespace { get; set; }

        public override int Invoke()
        {
            Console.WriteLine("here I Am");

            return 0;
        }
    }

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

    internal class NamespaceSuggester : ISuggester
    {
        public IEnumerable<string> GetSuggestions(ParseResult? parseResult = null, string? textToMatch = null)
        {
            return new List<string>() { "hello", "two"};
        }
    }

    [SubCommand("dotwoth", typeof(TestSubCommand2))]
    class TestSSububCommand2 : ConsoleCommand
    {

        [Option("--namespace", "-n", Description = "Something")]
        public string Namespace { get; set; }

        public override int Invoke()
        {
            Console.WriteLine("here I Am");

            return 0;
        }
    }
}