using System.Collections.Generic;
using System.CommandLine.Parsing;

namespace ConsoleFramework
{
    public interface ISuggester
    {
        IEnumerable<string> GetSuggestions(ParseResult? parseResult, string? textToMatch);
    }
}