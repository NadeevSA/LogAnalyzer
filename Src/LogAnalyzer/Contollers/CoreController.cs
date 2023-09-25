using AnalysisCore;
using Microsoft.AspNetCore.Mvc;

namespace LogAnalyzer.Contollers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class CoreController : Controller
    {
        [HttpPost("Calculation")]
        public async Task<double> GetCalculation([FromBody] Solution solution)
        {
            var result = await Core.Calculate(solution);
            return Math.Round(result, 2);
        }
    }
}
