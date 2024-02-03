using Core.Contracts;
using Microsoft.CodeAnalysis;
using Solution = Core.Contracts.Solution;

namespace Core.Services.Interfaces
{
    public interface IModule
    {
        void Handler(SyntaxNode syntaxNode, string filePath, Solution solution, ResultAnalysis resultAnalysis, HierarchyResult hierarchyResult);

        void AddResultTotal(ResultAnalysis resultAnalysis);
    }
}
