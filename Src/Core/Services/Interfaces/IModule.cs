using Core.Contracts;
using Microsoft.CodeAnalysis;
using Solution = Core.Contracts.Solution;

namespace Core.Services.Interfaces
{
    public interface IModule
    {
        void Handler(
            SyntaxNode syntaxNode,
            string filePath,
            string fullFulePath,
            Solution solution,
            ResultAnalysis resultAnalysis,
            HierarchyResult hierarchyResult,
            ChangeLoggers changeLoggers);

        void AddResultTotal(ResultAnalysis resultAnalysis);
    }
}
