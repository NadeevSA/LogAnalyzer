using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using static System.Net.Mime.MediaTypeNames;
using Text = DocumentFormat.OpenXml.Wordprocessing.Text;

namespace AnalysisCore
{
    public class Program
    {
        static string CompareTrees(Repository repo)
        {
            using (repo)
            {
                var commitTree = repo.Branches["refs/remotes/origin/test"].Tip.Tree; // Main Tree
                var resultString = new StringBuilder();
                foreach (var parent in repo.Head.Tip.Parents)
                {
                    var parentCommitTree = repo.Branches["refs/remotes/origin/master"].Tip.Tree;
                    var patch = repo.Diff.Compare<Patch>(parentCommitTree, commitTree); // Difference

                    foreach (var ptc in patch)
                    {
                        resultString.Append(ptc.Status + " -> " + ptc.Path + '\n');
                    }
                }

                return resultString.ToString();
            }
        }

        public static void Main()
        {
            //GIT
            var exampleRepositoryUrl = "https://github.com/NadeevSA/cicd.git";
            var exampleDestinationFolder = "src" + Guid.NewGuid();
            var exampleBranchName = "test";

            var branches = Repository.ListRemoteReferences(exampleRepositoryUrl)
                         .Where(elem => elem.IsLocalBranch)
                         .Select(elem => elem.CanonicalName
                                             .Replace("refs/heads/", ""));

            var repositoryClonedPath = Repository.Clone(exampleRepositoryUrl,
                                                        exampleDestinationFolder,
                                                        new CloneOptions()
                                                        {
                                                            BranchName = exampleBranchName
                                                        });
            var changes = CompareTrees(new Repository(repositoryClonedPath));
            
            ///WORD
            string templatePath = @"D:\Git\Forest\Templates\report.docx";
            string resultPath = @"D:\Git\Forest\Templates\report_result.docx";

            using (WordprocessingDocument document = WordprocessingDocument.CreateFromTemplate(templatePath))
            {
                var body = document.MainDocumentPart.Document.Body;
                var paragraphs = body.Elements<Paragraph>();
                foreach (Paragraph paragraph in paragraphs)
                {
                    foreach (Run run in paragraph.Elements<Run>())
                    {
                        foreach (Text text in run.Elements<Text>())
                        {
                            if (text.Text == "name_branch")
                            {
                                Console.WriteLine(text.Text);
                                text.Text = exampleBranchName;
                            }
                            if (text.Text == "change_in_branch")
                            {
                                Console.WriteLine(text.Text);
                                text.Text = changes;
                            }
                        }
                    }
                }
                document.Clone(resultPath);
            }
        }
    }
}
