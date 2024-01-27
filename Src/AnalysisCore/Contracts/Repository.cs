using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AnalysisCore.Contracts
{
    public class RepositoryJson
    {
        public string HierarchyFilesJson { get; set; }
        public string Path { set; get; }
    }
}
