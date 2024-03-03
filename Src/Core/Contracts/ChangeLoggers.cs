using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Core.Contracts
{
    public class ChangeLoggers
    {
        [JsonPropertyName("key")]
        public Guid Id { get; set; } = Guid.NewGuid();

        public string FilePath { get; set; } = string.Empty;

        public string FullFilePath { get; set; } = string.Empty;

        public int? LineNumber { get; set; } = null;

        [JsonPropertyName("children")]
        public List<ChangeLoggers> Children { get; set; } = null;

        public string OldCode { get; set; } = string.Empty;

        public string NewCode { get; set; } = string.Empty;

        public string PathRepo { get; set; } = string.Empty;

        public int CountChange { get; set; } = 0;
    }
}
