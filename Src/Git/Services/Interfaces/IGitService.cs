using Core.Contracts;
using System.Collections.Generic;

namespace Git.Services.Interfaces
{
    public interface IGitService
    {
        public List<string> GetBranchesByNameRepo(string repoName);

        public void PushBranch(IEnumerable<ChangeLoggers> changeLoggers, string nameNewBranch, string gitDescCommit);
    }
}
