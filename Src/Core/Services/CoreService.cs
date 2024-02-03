namespace Core.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.Json;
    using System.Threading.Tasks;
    using Core.Contracts;
    using Core.Services.Interfaces;
    using Core.Services.Modules;
    using Microsoft.Build.Locator;
    using Microsoft.CodeAnalysis.MSBuild;

    /// <summary>
    /// Основной сервис.
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

            var resultAnalysis = new ResultAnalysis();
            var modules = new List<IModule>();
            var hierarchyResultRoot = new HierarchyResult
            {
                Children = new List<HierarchyResult>(),
            };

            if (sourceSolution.IfElseChecker)
            {
                modules.Add(new IfElseModule());
            }

            foreach (var syntaxTree in compilation.SyntaxTrees)
            {
                if (!sourceSolution.CheckFiles.Contains(syntaxTree.FilePath))
                {
                    continue;
                }

                var filePath = syntaxTree.FilePath.Replace(
                    sourceSolution.NameFolder,
                    string.Empty);

                var root = await syntaxTree.GetRootAsync();
                foreach (var module in modules)
                {
                    module.Handler(root, filePath, sourceSolution, resultAnalysis, hierarchyResultRoot);
                }
            }

            foreach (var module in modules)
            {
                module.AddResultTotal(resultAnalysis);
            }

            var percentageLogs = resultAnalysis.IfElseLoggers > 0 ?
                (double)resultAnalysis.IfElseLoggers / resultAnalysis.AllCountLoggers * 100 : 0;

            resultAnalysis.PercentageLogs = percentageLogs;
            resultAnalysis.ResultJson = JsonSerializer.Serialize(hierarchyResultRoot.Children);
            resultAnalysis.ResultTotal = resultAnalysis.ResultTotalStringBuilder.ToString();

            return resultAnalysis;
        }
    }
}
