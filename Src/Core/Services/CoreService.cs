namespace Core.Services
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Text.Json;
    using System.Threading.Tasks;
    using Core.Contracts;
    using Core.Services.Interfaces;
    using Core.Services.Modules;
    using Microsoft.Build.Locator;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.FindSymbols;
    using Microsoft.CodeAnalysis.MSBuild;
    using Solution = Contracts.Solution;

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
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var visualStudioInstances = MSBuildLocator.QueryVisualStudioInstances().ToArray();
            var instance = visualStudioInstances[0];
            if (MSBuildLocator.CanRegister)
            {
                MSBuildLocator.RegisterInstance(instance);
            }

            var workspace = MSBuildWorkspace.Create();
            var solution = await workspace.OpenSolutionAsync(sourceSolution.Path);

            var resultAnalysis = new ResultAnalysis();
            var modules = new List<IModule>();
            var hierarchyResultRoot = new HierarchyResult
            {
                Children = new List<HierarchyResult>(),
            };
            var changEntityIdRoot = new ChangeLoggers
            {
                Children = new List<ChangeLoggers>(),
            };
            var changeEntityIdChilder = new ChangeLoggers
            {
                Children = new List<ChangeLoggers>(),
                FilePath = "Наименование сущностей",
            };
            changEntityIdRoot.Children.Add(changeEntityIdChilder);

            var changeLogs = new List<ChangeLog>();

            if (sourceSolution.IfElseChecker)
            {
                modules.Add(new MethodDeclarationModule());
            }

            foreach (var project in solution.Projects)
            {
                var compilation = await project.GetCompilationAsync();
                foreach (var syntaxTree in compilation.SyntaxTrees)
                {
                    if (!sourceSolution.CheckFiles.Contains(syntaxTree.FilePath))
                    {
                        continue;
                    }

                    var filePath = syntaxTree.FilePath.Replace(
                        sourceSolution.NameFolder,
                        string.Empty);
                    var fileName = syntaxTree.FilePath.Split(Path.DirectorySeparatorChar).Last();

                    var root = await syntaxTree.GetRootAsync();
                    foreach (var module in modules)
                    {
                        module.Handler(root, fileName, syntaxTree.FilePath, sourceSolution, resultAnalysis, hierarchyResultRoot, changeEntityIdChilder, changeLogs);
                    }
                }

                foreach (var module in modules)
                {
                    module.AddResultTotal(resultAnalysis);
                }
            }

            stopwatch.Stop();

            changeEntityIdChilder.FilePath = $"{changeEntityIdChilder.FilePath} ({changeEntityIdChilder.Children.Count})";
            foreach (var child in changeEntityIdChilder.Children)
            {
                child.FilePath = $"{child.FilePath} ({child.CountChange})";
            }
            changeEntityIdChilder.Children = changeEntityIdChilder.Children.OrderByDescending(c => c.CountChange).ToList();

            var percentageLogs = resultAnalysis.IfElseLoggers > 0 ?
                (double)resultAnalysis.IfElseLoggers / resultAnalysis.AllCountLoggers * 100 : 0;

            resultAnalysis.PercentageLogs = percentageLogs;
            resultAnalysis.ResultJson = JsonSerializer.Serialize(hierarchyResultRoot.Children);
            resultAnalysis.ChangeLoggersJson = JsonSerializer.Serialize(changEntityIdRoot.Children);
            resultAnalysis.ResultTotal = resultAnalysis.ResultTotalStringBuilder.ToString();
            resultAnalysis.TimeWork = (double)stopwatch.ElapsedMilliseconds / 1000;

            foreach (var changelogger in changeLogs)
            {
                string text = File.ReadAllText(changelogger.FullFilePath);
                text = text.Replace(changelogger.OldCode, changelogger.NewCode);
                File.WriteAllText(changelogger.FullFilePath, text);
            }

            GC.Collect();
            GC.WaitForPendingFinalizers();

            return resultAnalysis;
        }
    }
}
