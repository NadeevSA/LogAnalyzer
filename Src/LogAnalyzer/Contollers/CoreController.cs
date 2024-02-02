using Core.Contracts;
using Core.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace LogAnalyzer.Contollers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class CoreController : Controller
    {
        private readonly ICoreService _coreService;

        public CoreController(ICoreService coreService)
        {
            _coreService = coreService ?? throw new ArgumentException(nameof(coreService));
        }

        [HttpPost("Calculation")]
        public async Task<ResultAnalysis> GetCalculation([FromBody] Solution solution)
        {
            var result = await _coreService.CalculateAsync(solution);
            result.PercentageLogs = Math.Round(result.PercentageLogs, 2);

            return result;
        }
    }
}
