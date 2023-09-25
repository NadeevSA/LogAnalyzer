using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace AnalysisCore
{
    public class Program
    {
        static void CompareTrees(Repository repo)
        {
            using (repo)
            {
                foreach (var branch in repo.Branches) 
                {
                    Console.WriteLine(branch);
                }
                var commitTree = repo.Branches["refs/remotes/origin/test"].Tip.Tree; // Main Tree
                foreach (var parent in repo.Head.Tip.Parents)
                {
                    var parentCommitTree = repo.Branches["refs/remotes/origin/master"].Tip.Tree;
                    var patch = repo.Diff.Compare<Patch>(parentCommitTree, commitTree); // Difference

                    foreach (var ptc in patch)
                    {
                        Console.WriteLine(ptc.Status + " -> " + ptc.Path); // Status -> File Path
                    }
                }
            }
        }

        public static void Main()
        {
            var exampleRepositoryUrl = "https://github.com/NadeevSA/cicd.git";
            var exampleDestinationFolder = "src" + Guid.NewGuid();
            var exampleBranchName = "test";

            var branches = Repository.ListRemoteReferences(exampleRepositoryUrl)
                         .Where(elem => elem.IsLocalBranch)
                         .Select(elem => elem.CanonicalName
                                             .Replace("refs/heads/", ""));

            var repositoryClonedPath = Repository.Clone(exampleRepositoryUrl,
                                                        exampleDestinationFolder,
                                                        new CloneOptions()
                                                        {
                                                            BranchName = exampleBranchName
                                                        });

/*            var x = new Repository(repositoryClonedPath);
            foreach (var branch in branches)
            {
                Console.WriteLine(branch);
                var commits = x.Commits;
                List<string> shaCommits = new List<string>();
                string nextSha = commits.First().Sha;
                foreach (var commit in commits)
                {
                    if (nextSha == commit.Sha)
                    {
                        shaCommits.Add(commit.Sha);
                        nextSha = commit.Parents.First().Sha;
                    }
                }

            }*/

            CompareTrees(new Repository(repositoryClonedPath));
        }
    }
}
