using System;
using System.CommandLine.Parsing;
using System.Reflection;
using System.Threading.Tasks;
using ConsoleFramework;
using NUnit.Framework;
using Playground;

namespace Test
{
    public class Tests
    {

        [Test]
        public async Task Test1()
        {

            string[] args = { "--help"};

            await ConsoleApplicationBuilder
                .Create()
                .SetRootCommand<TestRootCommand>()
                .AddAllCommandsInAssembly(Assembly.GetAssembly(typeof(Program)) ?? throw new InvalidOperationException())
                .Build()
                .InvokeAsync(args);
        }
    }
}