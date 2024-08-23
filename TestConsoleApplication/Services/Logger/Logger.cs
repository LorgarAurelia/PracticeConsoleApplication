using TestConsoleApplication.Services.Repository;
using TestConsoleApplication.Services.Repository.Models;

namespace TestConsoleApplication.Services.Logger
{
    public class Logger(IBookCatalog db) : ITCLogger
    {
        public async Task LogBook(AnalyzeLog log) => await db.SaveLogAsync(log);
    }
}
