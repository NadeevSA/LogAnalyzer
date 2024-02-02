using Git.Contracts;
using Git.Services.Interfaces;
using LogAnalyzer.Contracts;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LogAnalyzer.Contollers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class GitController : Controller
    {
        private readonly IRepositoryService _repositoryService;
        private readonly IGitService _gitService;

        public GitController (IRepositoryService repositoryService, IGitService gitService) 
        {
            _repositoryService = repositoryService ?? throw new ArgumentException(nameof(repositoryService));
            _gitService = gitService ?? throw new ArgumentException(nameof(gitService));
        }

        [HttpPost("Repository")]
        public async Task<RepositoryJson> GetRepository([FromBody] InputData inputData)
        {
            (var path, var nameFolder) = _repositoryService.CreatePath(inputData);

            var hierarchyFilesJson = await _repositoryService.GetRepositoryAsync(nameFolder);
            var repository = new RepositoryJson()
            {
                HierarchyFilesJson = hierarchyFilesJson,
                Path = path,
            };
            return repository;
        }

        [HttpPost("BranchesByNameRepo")]
        public List<string> GetBranchesByNameRepo([FromBody] InputData inputData)
        {
            var result = _gitService.GetBranchesByNameRepo(inputData.NameRepo);
            return result;
        }
    }
}
