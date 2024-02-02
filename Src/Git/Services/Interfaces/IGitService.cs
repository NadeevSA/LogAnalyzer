using System.Collections.Generic;

namespace Git.Services.Interfaces
{
    public interface IGitService
    {
        public List<string> GetBranchesByNameRepo(string repoName);
    }
}
