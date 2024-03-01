using System;
using System.Collections.Generic;
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
            ChangeLoggers changeLoggersRoot)
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
            var changeLoggerList = new ChangeLoggers
            {
                FilePath = filePath,
            };

            foreach (var method in methodDeclarations)
            {
                var expression = method;
                var result = expression.ToFullString();
                var nameMethod = expression.Identifier.Text;
                var expressionStatement = expression.Body?
                    .DescendantNodes()
                    .OfType<Microsoft.CodeAnalysis.CSharp.Syntax.ExpressionStatementSyntax>()
                    .FirstOrDefault();
                var invocationExpression = expressionStatement?
                    .DescendantNodes()
                    .OfType<InvocationExpressionSyntax>()
                    .FirstOrDefault();
                var name = invocationExpression?.ToFullString();

                if (name != null && name.Contains(solution.NameLogger))
                {
                    var arguments = new List<string>();

                    var message = invocationExpression.ArgumentList.Arguments.FirstOrDefault();
                    var span = (int)invocationExpression.Span.Start;
                    var nameMethodArgument = AddStringArgument(nameMethod, span);
                    var classNameArgument = AddStringArgument(className.ToString(), span);
                    invocationExpression = invocationExpression.AddArgumentListArguments(nameMethodArgument, classNameArgument);

                    var x5 = message.DescendantNodes().OfType<Microsoft.CodeAnalysis.CSharp.Syntax.InterpolatedStringExpressionSyntax>().FirstOrDefault();
                    if (x5 != null)
                    {
                        var x6 = x5.DescendantNodes().OfType<Microsoft.CodeAnalysis.CSharp.Syntax.InterpolationSyntax>();
                        foreach (var x7 in x6)
                        {
                            var xxxx = x7.Expression.ToString();
                            if (x7.Expression.ToString().Contains("nameof") == false)
                            {
                                var argument = AddStringArgument(x7.ToFullString(), span);
                                invocationExpression = invocationExpression.AddArgumentListArguments(argument);
                            }
                        }
                    }

                    var newName = invocationExpression?.ToFullString();
                    ;
                }

                var x = expression.SyntaxTree.GetLineSpan(expression.Span);
                int lineNumber = x.StartLinePosition.Line + 1;

                var hierarchyResultChild = new HierarchyResult
                {
                    LineNumber = lineNumber,
                    Result = result,
                };

                logs++;
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
            if (changeLoggerList.Children != null)
            {
                changeLoggersRoot.Children.Add(changeLoggerList);
            }
        }

        private Microsoft.CodeAnalysis.CSharp.Syntax.ArgumentSyntax AddStringArgument(string argument, int lenWhiteSpace)
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
                                $"{argument}",
                                $"{argument}",
                                SyntaxFactory.TriviaList())))).
                            WithLeadingTrivia(SyntaxFactory.CarriageReturnLineFeed, SyntaxFactory.Whitespace(new string(' ', lenWhiteSpace))));

            return argumentSyntax;
        }
    }
}
