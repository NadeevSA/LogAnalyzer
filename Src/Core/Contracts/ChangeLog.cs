namespace Core.Contracts
{
    public class ChangeLog
    {
        public string FullFilePath { get; set; } = string.Empty;

        public string OldCode { get; set; } = string.Empty;

        public string NewCode { get; set; } = string.Empty;
    }
}
