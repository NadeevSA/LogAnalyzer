using Git.Services.Interfaces;
using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Git.Services
{
    public class GitService : IGitService
    {
        public List<string> GetBranchesByNameRepo(string repoName)
        {
            var branches = Repository.ListRemoteReferences(repoName)
             .Where(elem => elem.IsLocalBranch)
             .Select(elem => elem.CanonicalName
                                 .Replace("refs/heads/", ""));

            // TODO: Код для реализации автоматических правок в ветке.
/*            var branchName = $"LA-{Guid.NewGuid()}";
            using (var repo = new Repository("D:\\Git\\Forest\\Src\\LocalRepos\\-29c5a86f-1a3e-456e-a4d0-c53a7c49ec4a\\.git"))
            {
                Remote remote = repo.Network.Remotes["origin"];
                var branch = repo.CreateBranch(branchName);

                string gitUser = "nadeevSA", gitToken = "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";

                var options = new PushOptions
                {
                    CredentialsProvider = (_url, _user, _cred) => new UsernamePasswordCredentials { Username = gitUser, Password = gitToken },
                };

                var options1 = new CommitOptions
                {
                    AllowEmptyCommit = true,
                };

                repo.Branches.Update(branch,
                        b => b.Remote = remote.Name,
                        b => b.UpstreamBranch = branch.CanonicalName);

                Commands.Checkout(repo, branchName);

                var oldText = "_logger.Info($\"Get {string.Join(',', result)} through EF.\");";
                var newText = "_logger.Trace($\"Get {string.Join(',', result)} through EF.\");";
                var pathFile = "D:\\Git\\Forest\\Src\\LocalRepos\\-29c5a86f-1a3e-456e-a4d0-c53a7c49ec4a\\Services\\UserService.cs";
                string text = File.ReadAllText(pathFile);
                text = text.Replace(oldText, newText);
                File.WriteAllText(pathFile, text);

                Commands.Stage(repo, "*");
                repo.Commit("Create test commit...",
                    new Signature(gitUser, "nadeevSA@mail.ru", DateTimeOffset.Now),
                    new Signature(gitUser, "nadeevSA@mail.ru", DateTimeOffset.Now)
                    );


                repo.Network.Push(branch, options);
            }*/

            return branches.ToList();
        }
    }
}
