using TestConsoleApplication.Services.Repository.Models;

namespace TestConsoleApplication.Services.Logger
{
    public interface ITCLogger
    {
        Task LogBook(AnalyzeLog log);
    }
}