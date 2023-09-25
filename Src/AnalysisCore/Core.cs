using LibGit2Sharp;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;
using System;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AnalysisCore
{
    public static class Core
    {
        public static async Task<double> Calculate(Solution sourceSolution)
        {
            var visualStudioInstances = MSBuildLocator.QueryVisualStudioInstances().ToArray();
            var instance = visualStudioInstances[0];
            if (MSBuildLocator.CanRegister)
            {
                MSBuildLocator.RegisterInstance(instance);
            }

            var workspace = MSBuildWorkspace.Create();
            var solution = await workspace.OpenSolutionAsync(sourceSolution.Path);
            var project = solution.Projects.Single();
            var compilation = await project.GetCompilationAsync();
            var allIfElse = 0;
            var countLoggers = 0;

            foreach (var syntaxTree in compilation.SyntaxTrees)
            {
                var filePath = syntaxTree.FilePath;
                var fileName = System.IO.Path.GetFileNameWithoutExtension(filePath);
                if (sourceSolution.IgnoreFiles != null && sourceSolution.IgnoreFiles.Contains(fileName))
                {
                    continue;
                }

                var root = await syntaxTree.GetRootAsync();
                var model = compilation.GetSemanticModel(syntaxTree);
                var invocations = root.DescendantNodes().OfType<IfStatementSyntax>();
                foreach (var invocation in invocations)
                {
                    var expression = invocation;
                    var result = expression.ToFullString();
                    var x = expression.SyntaxTree.GetLineSpan(expression.Span);
                    int lineNumber = x.StartLinePosition.Line;
                    await Console.Out.WriteLineAsync(result);

                    allIfElse++;
                    countLoggers += IsLogger(result, sourceSolution.NameLogger);
                }
            }

            var percentageLogs = (double)countLoggers / allIfElse * 100;
            await Console.Out.WriteLineAsync($"Result: % log in code = {percentageLogs} " +
                $"[countLoggers = {countLoggers}, IfElseStatement = {allIfElse}]");

            return percentageLogs;
        }

        static int IsLogger(string sourceString, string nameLogger)
        {
            var result = sourceString.IndexOf(nameLogger);
            return result != -1 ? 1 : 0;
        }
    }
}
