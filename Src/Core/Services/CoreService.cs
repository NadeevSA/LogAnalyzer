namespace Core.Services
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.Contracts;
    using Core.Services.Interfaces;
    using Microsoft.Build.Locator;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Microsoft.CodeAnalysis.MSBuild;

    /// <summary>
    /// Оснвоной сервис.
    /// </summary>
    public class CoreService : ICoreService
    {
        /// <summary>
        /// Анализирует исходный код.
        /// </summary>
        public async Task<ResultAnalysis> CalculateAsync(Solution sourceSolution)
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
                if (!sourceSolution.CheckFiles.Contains(filePath))
                {
                    continue;
                }

                var root = await syntaxTree.GetRootAsync();
                var model = compilation.GetSemanticModel(syntaxTree);

                if (sourceSolution.IfElseChecker)
                {
                    var invocations = root.DescendantNodes().OfType<IfStatementSyntax>();

                    foreach (var invocation in invocations)
                    {
                        var expression = invocation;
                        var loc = expression.GetLocation();
                        var result = expression.ToFullString();
                        var x = expression.SyntaxTree.GetLineSpan(expression.Span);
                        int lineNumber = x.StartLinePosition.Line + 1;

                        await Console.Out.WriteLineAsync("fileName " + filePath);
                        await Console.Out.WriteLineAsync("lineNumber " + lineNumber);
                        await Console.Out.WriteLineAsync(result);

                        allIfElse++;
                        countLoggers += IsLogger(result, sourceSolution.NameLogger);
                    }
                }
            }

            var percentageLogs = allIfElse > 0 ?
                (double)countLoggers / allIfElse * 100 :
                0;

            await Console.Out.WriteLineAsync($"Result: % log in code = {percentageLogs} " +
                $"[countLoggers = {countLoggers}, IfElseStatement = {allIfElse}]");

            var resultAnalysis = new ResultAnalysis()
            {
                PercentageLogs = percentageLogs,
            };

            return resultAnalysis;
        }

        /// <summary>
        /// Определяет есть ли в строке логгер.
        /// </summary>
        private int IsLogger(string sourceString, string nameLogger)
        {
            var result = sourceString.IndexOf(nameLogger);
            return result != -1 ? 1 : 0;
        }
    }
}
