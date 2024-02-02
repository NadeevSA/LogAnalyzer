namespace LogAnalyzer.Contollers
{
    using System;
    using System.Threading.Tasks;
    using Core.Contracts;
    using Core.Services.Interfaces;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// Контроллер для работы с сервисом <seealso cref="ICoreService"/>.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class CoreController : Controller
    {
        private readonly ICoreService _coreService;

        /// <summary>
        /// Конструктор для <see cref="CoreController"/>.
        /// </summary>
        public CoreController(ICoreService coreService)
        {
            _coreService = coreService ?? throw new ArgumentException(nameof(coreService));
        }

        /// <summary>
        /// Возвращает результат анализа <seealso cref="ResultAnalysis"/>.
        /// </summary>
        [HttpPost("Calculation")]
        public async Task<ActionResult<ResultAnalysis>> GetCalculation([FromBody] Solution solution)
        {
            if (solution == null)
            {
                return BadRequest($"{nameof(Solution)} is not exist.");
            }

            var result = await _coreService.CalculateAsync(solution);
            result.PercentageLogs = Math.Round(result.PercentageLogs, 2);

            return result;
        }
    }
}
