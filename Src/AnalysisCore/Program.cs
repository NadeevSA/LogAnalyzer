using AnalysisCore.Contracts;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.IO;
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
                var commitTree = repo.Branches["refs/remotes/origin/TestingFront"].Tip.Tree; // Main Tree
                var resultString = new StringBuilder();
                /*                foreach (var parent in repo.Head.Tip.Parents)
                                {
                                    var parentCommitTree = repo.Branches["refs/remotes/origin/main"].Tip.Tree;
                                    var patch = repo.Diff.Compare<Patch>(parentCommitTree, commitTree); // Difference

                                    foreach (var ptc in patch)
                                    {
                                        resultString.Append(ptc.Status + " -> " + ptc.Path + '\n');
                                    }
                                }*/

                var parentCommitTree = repo.Branches["refs/remotes/origin/main"].Tip.Tree;
                //var patch = repo.Diff.Compare<Patch>((IEnumerable<string>)parentCommitTree); // Difference
                

                foreach (var ptc in parentCommitTree)
                {
                    //resultString.Append(ptc.Name + " -> " + ptc.Path + '\n');
                }

                RepositoryStatus status = repo.RetrieveStatus(
     new StatusOptions() { IncludeUnaltered = true | false });

                foreach (var item in status) //This only lists altered files
                {
                    resultString.Append(item.FilePath + '\n');
                }

                return resultString.ToString();
            }
        }

        public static void Main()
        {
            //GIT
            var exampleRepositoryUrl = "https://github.com/NadeevSA/TestForDiplom.git";
            var exampleDestinationFolder = "src-" + Guid.NewGuid();
            var exampleBranchName = "master";

            var branches = Repository.ListRemoteReferences(exampleRepositoryUrl)
                         .Where(elem => elem.IsLocalBranch)
                         .Select(elem => elem.CanonicalName
                                             .Replace("refs/heads/", ""));

            Console.WriteLine(string.Join(",", branches));

            var repositoryClonedPath = Repository.Clone(exampleRepositoryUrl,
                                                        exampleDestinationFolder,
                                                        new CloneOptions()
                                                        {
                                                            BranchName = exampleBranchName
                                                        });
            var path = Environment.CurrentDirectory + $"{Path.DirectorySeparatorChar}{exampleDestinationFolder}";
            Console.WriteLine(path);
            var solution = new Solution()
            {
                Path = path + $"{Path.DirectorySeparatorChar}OOP.sln",
                NameLogger = "_logger",
            };
            var x = Core.Calculate(solution).Result;
            Console.WriteLine(x);

            var directory = new DirectoryInfo(path) { Attributes = FileAttributes.Normal };
            foreach (var info in directory.GetFileSystemInfos("*", SearchOption.AllDirectories))
            {
                info.Attributes = FileAttributes.Normal;
            }

            directory.Delete(true);

            //var changes = CompareTrees(new Repository(repositoryClonedPath));

            ///WORD
            /*            string templatePath = @"D:\Git\Forest\Templates\report.docx";
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
                        }*/
        }
    }
}
