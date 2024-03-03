using Core.Contracts;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
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
            ChangeLoggers changeLoggers,
            List<ChangeLog> changeLogs);

        void AddResultTotal(ResultAnalysis resultAnalysis);
    }
}
