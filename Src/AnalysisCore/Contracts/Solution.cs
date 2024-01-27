using DocumentFormat.OpenXml.Office2010.ExcelAc;
using System.Collections.Generic;

namespace AnalysisCore.Contracts
{
    public class Solution
    {
        public string Path { get; set; }
        public string NameLogger { get; set; }
        public List<string> CheckFiles { get; set; } = new List<string>();
        public string NameBranch { get; set; }
        public string NameFolder { get; set; }
        public bool IfElseChecker { get; set; }
    }
}
