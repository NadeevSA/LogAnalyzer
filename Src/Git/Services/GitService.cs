using Core.Contracts;
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
                                 .Replace("refs/heads/", string.Empty));

            return branches.ToList();
        }

        public void PushBranch(IEnumerable<ChangeLoggers> changeLoggers, string nameNewBranch, string gitDescCommit)
        {
            using (var repo = new Repository($"{changeLoggers.First().PathRepo}{Path.DirectorySeparatorChar}.git"))
            {
                var remote = repo.Network.Remotes["origin"];
                var branch = repo.CreateBranch(nameNewBranch);

                string gitUser = "nadeevSA", gitToken = "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";

                var options = new PushOptions
                {
                    CredentialsProvider = (_url, _user, _cred) => new UsernamePasswordCredentials { Username = gitUser, Password = gitToken },
                };

                var options1 = new CommitOptions
                {
                    AllowEmptyCommit = false,
                };

                repo.Branches.Update(
                    branch,
                    b => b.Remote = remote.Name,
                    b => b.UpstreamBranch = branch.CanonicalName);

                Commands.Checkout(repo, nameNewBranch);

                foreach (var changelogger in changeLoggers)
                {
                    string text = File.ReadAllText(changelogger.FullFilePath);
                    text = text.Replace(changelogger.OldCode, changelogger.NewCode);
                    File.WriteAllText(changelogger.FullFilePath, text);
                }

                Commands.Stage(repo, "*");
                repo.Commit(
                    gitDescCommit ?? string.Empty,
                    new Signature(gitUser, "nadeevSA@mail.ru", DateTimeOffset.Now),
                    new Signature(gitUser, "nadeevSA@mail.ru", DateTimeOffset.Now));

                repo.Network.Push(branch, options);
            }
        }
    }
}
