using DocumentFormat.OpenXml.Presentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AnalysisCore.Contracts
{
    public class HierarchyFiles
    {
        [JsonPropertyName("key")]
        public Guid Id { get; set; }
        public string FileName { set; get; }
        [JsonPropertyName("children")]
        //[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<HierarchyFiles> Children { set; get; }

        [JsonPropertyName("chosen")]
        public bool Chosen { set; get; } = true;

        public bool IsDirectory { set; get; }

        public string Path { set; get; }
    }
}
