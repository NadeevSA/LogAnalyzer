﻿using AnalysisCore.Contracts;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Wordprocessing;
using LibGit2Sharp;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AnalysisCore
{
    public static class Core
    {
        public static async Task<ResultAnalysis> Calculate(Contracts.Solution sourceSolution)
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

        private static void DeleteEmptyDir(HierarchyFiles root)
        {
            var childs = root.Children.Where(c => c.IsDirectory);
            foreach (var child in childs)
            {
                DeleteEmptyDir(child);
            }
            root.Children = root.Children
                .Where(c => c.IsDirectory == false || (c.IsDirectory == true && c.Children.Count > 0))
                .ToList();
        }

        private static HierarchyFiles ReadFilesAndDirectories(DirectoryInfo directoryInfo, HierarchyFiles root)
        {
            foreach (var dir in directoryInfo.GetDirectories())
            {
                var childDirectoryItem = new HierarchyFiles
                {
                    Id = Guid.NewGuid(),
                    FileName = dir.Name,
                    Path = dir.FullName,
                    Children = new List<HierarchyFiles>(),
                    IsDirectory = true,
                };
                root.Children.Add(childDirectoryItem);
                ReadFilesAndDirectories(dir, childDirectoryItem);
            }

            foreach (var file in directoryInfo.GetFiles())
            {
                if (!file.Extension.Equals(".cs"))
                {
                    continue;
                }

                var childFileItem = new HierarchyFiles
                {
                    Id = Guid.NewGuid(),
                    FileName = file.Name,
                    Path = file.FullName,
                    IsDirectory = false,
                };
                root.Children.Add(childFileItem);
            }

            return root;
        }

        public static async Task<string> GetRepository(string pathFolder)
        {
            var dir = new DirectoryInfo(pathFolder);
            var root = new HierarchyFiles
            {
                Id = Guid.NewGuid(),
                FileName = "ROOT",
                Children = new List<HierarchyFiles>(),
            };
            var repository = ReadFilesAndDirectories(dir, root);
            DeleteEmptyDir(repository);

            string jsonString = JsonSerializer.Serialize(repository.Children);
            await Console.Out.WriteLineAsync(jsonString);

            return jsonString;
        }

        public static List<string> GetBranchesByNameRepo(string repoName)
        {
            var branches = Repository.ListRemoteReferences(repoName)
             .Where(elem => elem.IsLocalBranch)
             .Select(elem => elem.CanonicalName
                                 .Replace("refs/heads/", ""));

            return branches.ToList();
        }

        private static int IsLogger(string sourceString, string nameLogger)
        {
            var result = sourceString.IndexOf(nameLogger);
            return result != -1 ? 1 : 0;
        }

        public static (string, string) CreatePath(InputData inputData)
        {
            var pathSolution = SolutionProvider.TryGetSolutionDirectoryInfo();
            var pathLocalRepos = $"{pathSolution}{Path.DirectorySeparatorChar}LocalRepos";

            if (Directory.Exists(pathLocalRepos))
            {
                var directory = new DirectoryInfo(pathLocalRepos)
                { Attributes = FileAttributes.Normal };

                foreach (var info in directory.GetFileSystemInfos("*", SearchOption.AllDirectories))
                {
                    info.Attributes = FileAttributes.Normal;
                }

                directory.Delete(true);
            }

            //var exampleRepositoryUrl = "https://github.com/NadeevSA/TestForDiplom.git";

            var exampleDestinationFolder =
                $"{pathSolution}{Path.DirectorySeparatorChar}" +
                $"LocalRepos{Path.DirectorySeparatorChar}" +
                $"{inputData.NameBranch}-{Guid.NewGuid()}";

            Repository.Clone(inputData.NameRepo,
                         exampleDestinationFolder,
                         new CloneOptions()
                         {
                             BranchName = inputData.NameBranch
                         });

            var path = $"{exampleDestinationFolder}{Path.DirectorySeparatorChar}{inputData.NameSln}.sln";
            return (path, exampleDestinationFolder);
        }
    }
}
