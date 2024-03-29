﻿namespace Core.Contracts
{
    using System.Collections.Generic;

    /// <summary>
    /// Решение.
    /// </summary>
    public class Solution
    {
        public string Path { get; set; }

        public string NameLogger { get; set; }

        public List<string> CheckFiles { get; set; } = new List<string>();

        public string NameFolder { get; set; }

        public bool IfElseChecker { get; set; }
    }
}
