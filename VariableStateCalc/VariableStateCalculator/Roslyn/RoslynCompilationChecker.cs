using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace VariableStateCalculator.Roslyn
{
    internal static class RoslynCompilationChecker
    {
        public static void CheckErrors(SyntaxTree tree)
        {
            CSharpCompilation compilation = CSharpCompilation.Create("VariableStateSourceApp")
                .AddReferences(MetadataReference.CreateFromFile(typeof(String).Assembly.Location))
                .AddSyntaxTrees(tree)
                .WithOptions(new CSharpCompilationOptions(OutputKind.ConsoleApplication));
            Boolean hasErrors = compilation.GetDiagnostics().Any(d => d.Severity == DiagnosticSeverity.Error);
            if (hasErrors)
                throw new InvalidOperationException();
        }
    }
}
