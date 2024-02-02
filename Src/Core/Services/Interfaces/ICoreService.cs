using Core.Contracts;
using System.Threading.Tasks;

namespace Core.Services.Interfaces
{
    public interface ICoreService
    {
        Task<ResultAnalysis> CalculateAsync(Solution sourceSolution);
    }
}
