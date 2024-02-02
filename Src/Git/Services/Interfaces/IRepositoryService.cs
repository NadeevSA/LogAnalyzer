using Git.Contracts;
using System.Threading.Tasks;

namespace Git.Services.Interfaces
{
    public interface IRepositoryService
    {
        public Task<string> GetRepositoryAsync(string pathFolder);
        public (string, string) CreatePath(InputData inputData);
    }
}
