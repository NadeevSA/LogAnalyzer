using Core.Contracts;
using Core.Services.Interfaces;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Solution = Core.Contracts.Solution;

namespace Core.Services.Modules
{
    public class IfElseModule : BaseModule, IModule
    {
        private const string NAMEMODULE = "IF-ELSE";
        private readonly IModule _module;

        public IfElseModule(IModule module = null)
        {
            _module = module;
        }

        public async void Handler(
            SyntaxNode syntaxNode,
            string filePath,
            Solution solution,
            ResultAnalysis resultAnalysis,
            HierarchyResult hierarchyResultRoot)
        {
            var invocations = syntaxNode.DescendantNodes().OfType<IfStatementSyntax>();
            var allIfElse = 0;
            var hierarchyResultList = new List<HierarchyResult>();

            foreach (var invocation in invocations)
            {
                var expression = invocation;
                var result = expression.ToFullString();
                var x = expression.SyntaxTree.GetLineSpan(expression.Span);
                int lineNumber = x.StartLinePosition.Line + 1;

                var hierarchyResultChild = new HierarchyResult
                {
                    LineNumber = lineNumber,
                    Result = result,
                };

                allIfElse++;
                resultAnalysis.IfElseLoggers += IsLogger(result, solution.NameLogger, hierarchyResultChild);
                hierarchyResultList.Add(hierarchyResultChild);
            }

            if (hierarchyResultList.Any())
            {
                var hierarchyResultChild = new HierarchyResult
                {
                    FilePath = filePath,
                    Children = hierarchyResultList,
                };
                hierarchyResultRoot.Children.Add(hierarchyResultChild);
            }

            resultAnalysis.AllCountLoggers += allIfElse;
        }

        public void AddResultTotal(ResultAnalysis resultAnalysis)
        {
            resultAnalysis.ResultTotalStringBuilder.AppendLine($"для констукции [{NAMEMODULE}] = " +
                $"{resultAnalysis.IfElseLoggers}/{resultAnalysis.AllCountLoggers};");
        }
    }
}
