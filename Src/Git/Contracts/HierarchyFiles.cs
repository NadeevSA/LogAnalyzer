using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Git.Contracts
{
    public class HierarchyFiles
    {
        [JsonPropertyName("key")]
        public Guid Id { get; set; }

        public string FileName { set; get; }

        [JsonPropertyName("children")]
        public List<HierarchyFiles> Children { set; get; }

        [JsonPropertyName("chosen")]
        public bool Chosen { set; get; } = true;

        public bool IsDirectory { set; get; }

        public string Path { set; get; }
    }
}
