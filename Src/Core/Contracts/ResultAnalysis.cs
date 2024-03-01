using System;
using System.Text;

namespace Core.Contracts
{
    public class ResultAnalysis
    {
        public double PercentageLogs { get; set; }

        public int AllCountLoggers { get; set; }

        public int IfElseLoggers { get; set; }

        public StringBuilder ResultTotalStringBuilder { get; set; } = new StringBuilder($"Результаты:{Environment.NewLine}");

        public string ResultTotal { get; set; }

        public string ResultJson { get; set; }

        public string ChangeLoggersJson { get; set; }

        public double TimeWork { get; set; }
    }
}
