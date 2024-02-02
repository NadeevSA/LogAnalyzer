namespace Core.Services.Interfaces
{
    using System.Threading.Tasks;
    using Core.Contracts;

    /// <summary>
    /// Интерфейс для core-сервиса.
    /// </summary>
    public interface ICoreService
    {
        Task<ResultAnalysis> CalculateAsync(Solution sourceSolution);
    }
}
