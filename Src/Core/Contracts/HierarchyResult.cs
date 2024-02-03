using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Core.Contracts
{
    public class HierarchyResult
    {
        [JsonPropertyName("key")]
        public Guid Id { get; set; } = Guid.NewGuid();

        public string FilePath { get; set; } = string.Empty;

        public int? LineNumber { get; set; } = null;

        public string Result { get; set; } = string.Empty;

        public string ResultText { get; set; } = string.Empty;

        [JsonPropertyName("children")]
        public List<HierarchyResult> Children { get; set; } = null;
    }
}
