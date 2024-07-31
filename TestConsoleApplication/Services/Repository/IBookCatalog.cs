using TestConsoleApplication.Services.Repository.Models;

namespace TestConsoleApplication.Services.Repository
{
    public interface IBookCatalog
    {
        Task<Book[]> GetAllBoksAsync();
        Task<Book[]> GetUnfinishedBooksAsync(int lastId);
        Task<AnalyzeLog[]> GetUnfinishedTasksAsync();
        Task SaveBooksAsync(IEnumerable<Book> books);
        Task SaveLogAsync(AnalyzeLog log);
    }
}