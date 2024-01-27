using AnalysisCore;
using AnalysisCore.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace LogAnalyzer.Contollers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class CoreController : Controller
    {
        [HttpPost("Calculation")]
        public async Task<ResultAnalysis> GetCalculation([FromBody] Solution solution)
        {
            var result = await Core.Calculate(solution);
            result.PercentageLogs = Math.Round(result.PercentageLogs, 2);

            return result;
        }

        [HttpPost("Repository")]
        public async Task<RepositoryJson> GetRepository([FromBody] InputData inputData)
        {
            (var path, var nameFolder) = Core.CreatePath(inputData);

            var hierarchyFilesJson = await Core.GetRepository(nameFolder);
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
            var result = Core.GetBranchesByNameRepo(inputData.NameRepo);
            return result;
        }
    }
}
