using System;
using System.Collections.Generic;
using System.IO;
using VariableStateCalculator;
using VariableStateCalculator.Antlr;
using VariableStateCalculator.Roslyn;

namespace VariableStateCalcApp
{
    class Program
    {
        static void Main(String[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine(AppDescription);
                return;
            }
            if (args.Length == 1 && args[0] == HelpOption)
            {
                Console.WriteLine(AppDescription);
                return;
            }
            if (args.Length == 1 && args[0] == VersionOption)
            {
                Console.WriteLine(VersionNumber);
                return;
            }
            if (args.Length == 1)
            {
                ProcessSource(args[0], KnownCalculators[DefaultCalculator]());
                return;
            }
            if (args.Length == 3 && args[0] == CalculatorOption)
            {
                String calculatorKey = args[1];
                if (KnownCalculators.ContainsKey(calculatorKey))
                    ProcessSource(args[2], KnownCalculators[calculatorKey]());
                else
                    Console.Error.WriteLine(UnknownCalculatorMessage);
                return;
            }
            Console.Error.WriteLine(BadUsageMessage);
            Console.WriteLine(AppDescription);
        }

        private static void ProcessSource(String sourcePath, IVariableStateCalculator calculator)
        {
            Int32[] states = calculator.Calculate(File.ReadAllText(sourcePath));
            Console.WriteLine("Result: {0}", String.Join(", ", states));
        }

        private static readonly IDictionary<String, Func<IVariableStateCalculator>> KnownCalculators = new Dictionary<String, Func<IVariableStateCalculator>>
        {
            {AntlrCalculator, () => new VariableStateAntlrCalculator()},
            {RoslynDirectCalculator, () => new VariableStateRoslynDirectCalculator()},
            {RoslynWalkerCalculator, () => new VariableStateRoslynWalkerCalculator()}
        };

        private const String CalculatorOption = "--calculator";
        private const String HelpOption = "--help";
        private const String VersionOption = "--version";
        private const String AntlrCalculator = "Antlr";
        private const String RoslynDirectCalculator = "RoslynDirect";
        private const String RoslynWalkerCalculator = "RoslynWalker";
        private const String DefaultCalculator = AntlrCalculator;
        private const String AppDescription = "Application usage:\r\n" +
                                              "1. {APP} [--calculator {Antlr(default)|RoslynDirect|RoslynWalker}] {source-filename.cs}\r\n" +
                                              "2. {APP} --help\r\n" +
                                              "3. {APP} --version";
        private const String BadUsageMessage = "Bad usage of the application.";
        private const String UnknownCalculatorMessage = "Attempt of unknown calculator using.";
        private const String VersionNumber = "0.1";
    }
}
