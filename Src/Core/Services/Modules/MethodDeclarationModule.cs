using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text.RegularExpressions;
using Core.Contracts;
using Core.Services.Interfaces;
using Microsoft.Build.Framework.XamlTypes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using ArgumentListSyntax = Microsoft.CodeAnalysis.CSharp.Syntax.ArgumentListSyntax;
using ArgumentSyntax = Microsoft.CodeAnalysis.CSharp.Syntax.ArgumentSyntax;
using InterpolatedStringContentSyntax = Microsoft.CodeAnalysis.CSharp.Syntax.InterpolatedStringContentSyntax;
using InvocationExpressionSyntax = Microsoft.CodeAnalysis.CSharp.Syntax.InvocationExpressionSyntax;

namespace Core.Services.Modules
{
    public class MethodDeclarationModule : BaseModule, IModule
    {
        public void AddResultTotal(ResultAnalysis resultAnalysis)
        {
            resultAnalysis.ResultTotalStringBuilder.AppendLine($"Общее количество логов = " +
                $"{resultAnalysis.AllCountLoggers};");
        }

        public void Handler(
            SyntaxNode syntaxNode,
            string filePath,
            string fullFilePath,
            Contracts.Solution solution,
            ResultAnalysis resultAnalysis,
            HierarchyResult hierarchyResultRoot,
            ChangeLoggers changeClassNameRoot)
        {
            var methodDeclarations = syntaxNode.DescendantNodes().OfType<MethodDeclarationSyntax>();
            var classDeclarationSyntax = syntaxNode.DescendantNodes().OfType<ClassDeclarationSyntax>().FirstOrDefault();
            object className = null;
            if (classDeclarationSyntax != null)
            {
                className = classDeclarationSyntax.Identifier.Value;
            }

            var logs = 0;
            var hierarchyResultList = new List<HierarchyResult>();
            var changeLoggers = new List<ChangeLoggers>();
            var numberLineLogs = new List<int>();
            var changeLoggerList = new ChangeLoggers
            {
                FilePath = filePath,
            };
            var changesClassNmame = new ChangeLoggers
            {
                FilePath = "Наименование класса",
            };

            foreach (var method in methodDeclarations)
            {
                var expression = method;
                var result = expression.ToFullString();
                var nameMethod = expression.Identifier.Text;
                var expressionStatements = expression.Body?
                    .DescendantNodes()
                    .OfType<Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionStatementSyntax>();
                if (expressionStatements == null)
                {
                    continue;
                }

                foreach (var expressionStatement in expressionStatements)
                {
                    var invocationExpressions = expressionStatement?
                    .DescendantNodes()
                    .OfType<InvocationExpressionSyntax>();
                    if (invocationExpressions == null)
                    {
                        continue;
                    }

                    foreach (var invocationExpression in invocationExpressions)
                    {
                        var name = invocationExpression?.ToFullString();
                        var expressionName = invocationExpression.Expression.ToString();
                        var curInvocationExpression = invocationExpression;
                        var x = syntaxNode.SyntaxTree.GetLineSpan(curInvocationExpression.Span);
                        int lineNumber = x.StartLinePosition.Line + 1;

                        if (expressionName != null &&
                            numberLineLogs.Contains(lineNumber) == false &&
                            expressionName.Contains(solution.NameLogger))
                        {
                            var arguments = new List<string>();
                            var argList = SyntaxFactory.ArgumentList();

                            var message = invocationExpression.ArgumentList.Arguments.FirstOrDefault();
                            if (message == null)
                            {
                                continue;
                            }

                            var x10 = expression.SyntaxTree.GetLineSpan(message.Span);
                            int span = x10.StartLinePosition.Character;

                            var classNameArgument = AddStringArgument($@"{{nameof({className})}}", span, true);
                            var nameMethodArgument = AddStringArgument($@"{{nameof({className}.{nameMethod})}}", span);

                            argList = argList.AddArguments(classNameArgument, nameMethodArgument);
                            foreach (var arg in invocationExpression.ArgumentList.Arguments)
                            {
                                argList = argList.AddArguments(arg.
                                    WithLeadingTrivia(SyntaxFactory.CarriageReturnLineFeed, SyntaxFactory.Whitespace(new string(' ', span))));
                            }

                            var x5 = message.DescendantNodes().OfType<Microsoft.CodeAnalysis.CSharp.Syntax.InterpolatedStringExpressionSyntax>().FirstOrDefault();
                            if (x5 != null)
                            {
                                var x6 = x5.DescendantNodes().OfType<Microsoft.CodeAnalysis.CSharp.Syntax.InterpolationSyntax>();
                                foreach (var x7 in x6)
                                {
                                    if (x7.Expression.ToString().Contains("nameof") == false)
                                    {
                                        var changeLogger = new ChangeLoggers
                                        {
                                            FilePath = filePath,
                                            LineNumber = lineNumber,
                                            FullFilePath = fullFilePath,
                                            NewCode = string.Empty,
                                            OldCode = string.Empty,
                                            PathRepo = solution.NameFolder,
                                        };
                                        var x8 = UpdateString(x7.ToString());
                                        if (changeClassNameRoot.Children.Select(c => c.FilePath).ToList().Contains(x8) == true)
                                        {
                                            var x123 = changeClassNameRoot.Children.Where(c => c.FilePath == x8).First();
                                            x123.Children.Add(changeLogger);
                                            x123.CountChange++;
                                        }
                                        else
                                        {
                                            var changeLogger1 = new ChangeLoggers
                                            {
                                                FilePath = x8,
                                                FullFilePath = string.Empty,
                                                NewCode = string.Empty,
                                                OldCode = string.Empty,
                                                PathRepo = solution.NameFolder,
                                            };
                                            changeClassNameRoot.Children.Add(changeLogger1);
                                            changeLogger1.Children = new List<ChangeLoggers> { changeLogger };
                                            changeLogger1.CountChange = 1;
                                        }
                                        var argument = AddStringArgument($"{{nameof({x7})}}-{x7}", span);
                                        argList = argList.AddArguments(argument);
                                    }
                                }
                            }

                            curInvocationExpression = curInvocationExpression.ReplaceNode(curInvocationExpression.ArgumentList, argList);

                            var newCode = curInvocationExpression?.ToFullString();

                            logs++;
                            numberLineLogs.Add(lineNumber);
                        }
                    }
                }

                var hierarchyResultChild = new HierarchyResult
                {
                    LineNumber = 0,
                    Result = result,
                };
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

            resultAnalysis.AllCountLoggers += logs;

            //GC.Collect();
            //GC.WaitForPendingFinalizers();
            /*            if (changeLoggerList.Children != null)
                        {
                            changeLoggersRoot.Children.Add(changeLoggers);
                        }*/
        }

        private ArgumentSyntax AddStringArgument(string argument, int lenWhiteSpace, bool first = false)
        {
            var argumentSyntax = SyntaxFactory.Argument(
            SyntaxFactory.InterpolatedStringExpression(
                SyntaxFactory.Token(SyntaxKind.InterpolatedStringStartToken))
                .WithContents(
                    SyntaxFactory.SingletonList<InterpolatedStringContentSyntax>(
                        SyntaxFactory.InterpolatedStringText()
                        .WithTextToken(
                            SyntaxFactory.Token(
                                SyntaxFactory.TriviaList(),
                                SyntaxKind.InterpolatedStringTextToken,
                                argument,
                                argument,
                                SyntaxFactory.TriviaList())))));

            if (first == false)
            {
                argumentSyntax = argumentSyntax.WithLeadingTrivia(SyntaxFactory.CarriageReturnLineFeed, SyntaxFactory.Whitespace(new string(' ', lenWhiteSpace)));
            }

            return argumentSyntax;
        }

        private string UpdateString(string value)
        {
            var result = value.Substring(1, value.Length - 2);
            result = result.ToLower();
            result = Replace(result, "role", "role");
            result = Replace(result, "request", "request");
            result = Replace(result, "response", "response");
            result = Replace(result, "workorder", "workorder");
            result = Replace(result, "event", "event");
            result = Replace(result, "state", "state");
            result = Replace(result, "exception", "error");
            result = Replace(result, "exception", "exception");
            result = Replace(result, "name", "name");
            result = Replace(result, "id", "id");
            if (result == "e")
            {
                result = "exception";
            }
            if (result == "ex")
            {
                result = "exception";
            }

            return result;
        }

        private string Replace(string value, string replacement1, string replacement2)
        {
            if (value.Contains(replacement1) || value.Contains(replacement2))
            {
                value = replacement1;
            }

            return value;
        }
    }
}
