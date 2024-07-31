using TestConsoleApplication.Services.Logger;
using TestConsoleApplication.Services.Repository.Models;

namespace TestConsoleApplicationTests.Environment
{
    public class LoggerForTests : ITCLogger
    {
        public List<AnalyzeLog> Logs { get; set; } = new();
        public async Task LogBook(AnalyzeLog log) => await Task.Run(() => { Logs.Add(log); });
    }
}
