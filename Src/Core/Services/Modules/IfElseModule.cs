using Core.Contracts;
using Core.Services.Interfaces;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.FindSymbols;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using ArgumentSyntax = Microsoft.CodeAnalysis.CSharp.Syntax.ArgumentSyntax;
using ExpressionStatementSyntax = Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionStatementSyntax;
using InterpolatedStringContentSyntax = Microsoft.CodeAnalysis.CSharp.Syntax.InterpolatedStringContentSyntax;
using Solution = Core.Contracts.Solution;
using SyntaxFactory = Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using SyntaxKind = Microsoft.CodeAnalysis.CSharp.SyntaxKind;

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
            ChangeLoggers changeLoggersRoot,
            List<ChangeLog> changeLogs)
        {
            var methodDeclarations = syntaxNode.DescendantNodes().OfType<MethodDeclarationSyntax>();
            var x1 = syntaxNode.DescendantNodes(n => n.IsKind(SyntaxKind.IfStatement)).ToList();
            var allIfElse = 0;
            var hierarchyResultList = new List<HierarchyResult>();
            var changeLoggerList = new ChangeLoggers
            {
                FilePath = filePath,
            };

            foreach (var invocation in methodDeclarations)
            {
                var expression = invocation;
                var result = expression.ToFullString();
/*                //MethodDeclarationSyntax
                var nameMethod = expression.Identifier.Text;
                //var ifStatement = expression.Body?.DescendantNodes().OfType<IfStatementSyntax>().FirstOrDefault();
                var expressionStatement = expression.Body?
                    .DescendantNodes()
                    .OfType<Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionStatementSyntax>()
                    .FirstOrDefault();
                var invocationExpression = expressionStatement?
                    .DescendantNodes()
                    .OfType<Microsoft.CodeAnalysis.CSharp.Syntax.InvocationExpressionSyntax>()
                    .FirstOrDefault();
                var name = invocationExpression?.ToFullString();*/

/*                if (name != null && name.Contains(solution.NameLogger))
                {
                    var x5 = SyntaxFactory.Argument(
                                SyntaxFactory.InterpolatedStringExpression(
                                    SyntaxFactory.Token(SyntaxKind.InterpolatedStringStartToken))
                                .WithContents(
                                    SyntaxFactory.SingletonList<InterpolatedStringContentSyntax>(
                                        SyntaxFactory.InterpolatedStringText()
                                        .WithTextToken(
                                            SyntaxFactory.Token(
                                                SyntaxFactory.TriviaList(),
                                                SyntaxKind.InterpolatedStringTextToken,
                                                $"{nameMethod}",
                                                $"{nameMethod}",
                                                SyntaxFactory.TriviaList())))));


                    invocationExpression = invocationExpression.AddArgumentListArguments(x5);
                    var newName = invocationExpression?.ToFullString();
                    ;
                }*/

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
                        if (changeLoggerList.Children == null)
                        {
                            changeLoggerList.Children = new List<ChangeLoggers>();
                        }

                        changeLoggerList.Children.Add(changeLogger);
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
            if (changeLoggerList.Children != null)
            {
                changeLoggersRoot.Children.Add(changeLoggerList);
            }
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
