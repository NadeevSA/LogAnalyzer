namespace LogAnalyzer.Contracts
{
    /// <summary>
    /// Репозиторий.
    /// </summary>
    public class RepositoryJson
    {
        /// <summary>
        /// Иерархия файлов в репозитории.
        /// </summary>
        public string HierarchyFilesJson { get; set; }

        /// <summary>
        /// Путь до файла.
        /// </summary>
        public string Path { get; set; }

        public string NameFolder { get; set; }
    }
}
