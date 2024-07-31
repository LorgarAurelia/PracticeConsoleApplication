namespace TestConsoleApplication.Common
{
    public static class ExceptionsMessages
    {
        #region StartApplication

        public const string TextAnalyzerMissing = "textAnalyzerSetting.json is missing";
        public const string ClusterSyzeRule = "Size of cluster must be more than zero";
        public const string KeywordMissing = "You must set keyword for search";

        #endregion

        #region UI

        public const string IncorrectDataType = "Expected another data type";

        #endregion

        #region FilesSystem

        public const string CorruptedExportStorage = "Content of export storage does not satisfy format";
        public const string FileControllerDosentInitialized = "You need initialize FileController by EnsureFilesSystemCreatedAsync() method";

        #endregion
    }
}
