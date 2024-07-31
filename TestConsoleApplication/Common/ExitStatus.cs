namespace TestConsoleApplication.Common
{
    public enum ExitStatus : int
    {
        NotRequired = -1,
        Success = 0,
        FileSystemException = 1,
        RequiredByUser = 2,
        StartupException = 3,
    }
}
