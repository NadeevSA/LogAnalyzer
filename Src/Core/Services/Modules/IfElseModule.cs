using Core.Contracts;
using Core.Services.Interfaces;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.FindSymbols;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Solution = Core.Contracts.Solution;

namespace Core.Services.Modules
{
    public class IfElseModule : BaseModule, IModule
    {
        private const string NAMEMODULE = "IF-ELSE";

        public async void Handler(
            SyntaxNode syntaxNode,
            string filePath,
            string fullFilePath,
            Solution solution,
            ResultAnalysis resultAnalysis,
            HierarchyResult hierarchyResultRoot,
            ChangeLoggers changeLoggersRoot)
        {
            var invocations = syntaxNode.DescendantNodes().OfType<IfStatementSyntax>();
            var x1 = syntaxNode.DescendantNodes(n => n.IsKind(SyntaxKind.IfStatement)).ToList();
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
                var isLogger = IsLogger(result, solution.NameLogger, hierarchyResultChild);
                resultAnalysis.IfElseLoggers += isLogger;
                hierarchyResultList.Add(hierarchyResultChild);

                if (isLogger == 1)
                {
                    var message = Regex.Match(result, "\\\"(.*?)\\\"").Value.Trim('"');
                    var (resultChanged, newCode) = ChangeLog(message);

                    if (resultChanged)
                    {
                        var changeLogger = new ChangeLoggers
                        {
                            FilePath = filePath,
                            LineNumber = lineNumber,
                            FullFilePath = fullFilePath,
                            NewCode = newCode,
                            OldCode = message,
                            PathRepo = solution.NameFolder,
                        };
                        changeLoggersRoot.Children.Add(changeLogger);
                    }
                }
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

        private (bool, string) ChangeLog(string message)
        {
            var resultChanged = false;

            var pattern = "\\.$";
            if (!Regex.IsMatch(message, pattern))
            {
                message += '.';
                resultChanged = true;
            }

            var p1 = "^[^A-Z]";
            if (Regex.IsMatch(message, p1))
            {
                message = char.ToUpper(message[0]) + message.Substring(1);
                resultChanged = true;
            }

            return (resultChanged, message);
        }
    }
}
