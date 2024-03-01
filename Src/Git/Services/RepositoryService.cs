﻿using Git.Contracts;
using Git.Providers;
using Git.Services.Interfaces;
using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Git.Services
{
    public class RepositoryService : IRepositoryService
    {
        public async Task<string> GetRepositoryAsync(string pathFolder)
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

        private HierarchyFiles ReadFilesAndDirectories(DirectoryInfo directoryInfo, HierarchyFiles root)
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

        private void DeleteEmptyDir(HierarchyFiles root)
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

        public (string, string) CreatePath(InputData inputData)
        {
            var pathSln = "D:\\Work\\uberizationpurchases";
            //var pathSolution = SolutionProvider.TryGetSolutionDirectoryInfo(pathSln);
            var pathSolution = "D:\\Work\\uberizationpurchases\\src\\GpnDs.UBER.sln";
            var pathLocalRepos = $"{pathSolution}";

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

/*            Repository.Clone(inputData.NameRepo,
                         exampleDestinationFolder,
                         new CloneOptions()
                         {
                             BranchName = inputData.NameBranch
                         });*/

            var path = $"{exampleDestinationFolder}{Path.DirectorySeparatorChar}{inputData.NameSln}";
            return (pathSolution, "D:\\Work\\uberizationpurchases\\src");
        }
    }
}
