namespace LogAnalyzer.Contollers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Git.Contracts;
    using Git.Services.Interfaces;
    using LogAnalyzer.Contracts;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// Контроллер для работы сервисом <see cref="IGitService"/>.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class GitController : Controller
    {
        private readonly IRepositoryService _repositoryService;
        private readonly IGitService _gitService;

        /// <summary>
        /// Констуктор для <see cref="GitController"/>.
        /// </summary>
        public GitController(IRepositoryService repositoryService, IGitService gitService)
        {
            _repositoryService = repositoryService ?? throw new ArgumentException(nameof(repositoryService));
            _gitService = gitService ?? throw new ArgumentException(nameof(gitService));
        }

        /// <summary>
        /// Возвращает <see cref="RepositoryJson"/> для вывода иерархии на ЭФ.
        /// </summary>
        [HttpPost("Repository")]
        public async Task<RepositoryJson> GetRepository([FromBody] InputData inputData)
        {
            (var path, var nameFolder) = _repositoryService.CreatePath(inputData);

            var hierarchyFilesJson = await _repositoryService.GetRepositoryAsync(nameFolder);
            var repository = new RepositoryJson()
            {
                HierarchyFilesJson = hierarchyFilesJson,
                Path = path,
                NameFolder = nameFolder,
            };
            return repository;
        }

        /// <summary>
        /// Возвращает коллекцию веток по названию репозитория.
        /// </summary>
        [HttpPost("BranchesByNameRepo")]
        public List<string> GetBranchesByNameRepo([FromBody] InputData inputData)
        {
            var result = _gitService.GetBranchesByNameRepo(inputData.NameRepo);
            return result;
        }
    }
}
