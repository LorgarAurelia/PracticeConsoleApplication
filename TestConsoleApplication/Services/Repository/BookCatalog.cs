using Microsoft.EntityFrameworkCore;
using TestConsoleApplication.Services.Logger;
using TestConsoleApplication.Services.Repository.Models;

namespace TestConsoleApplication.Services.Repository
{
    public class BookCatalog : IBookCatalog
    {
        private readonly BookContext _context;
        private int _counter = 0;

        public BookCatalog(BookContext context) => _context = context;
        //TODO: Make it use TCResult
        public async Task SaveLogAsync(AnalyzeLog log)
        {
            _context.Logs.Add(log);
            await _context.SaveChangesAsync();
        }
        public async Task<AnalyzeLog[]> GetUnfinishedTasksAsync() => await _context.Logs.AsNoTracking().Where(u => u.Status == AnalyzeStatus.Stoped).ToArrayAsync();

        public async Task SaveBooksAsync(IEnumerable<Book> books)
        {
            await _context.AddRangeAsync(books);
            await _context.SaveChangesAsync();
        }
        public async Task<Book[]> GetAllBoksAsync() => await _context.Books.AsNoTracking().ToArrayAsync();
        public async Task<Book[]> GetUnfinishedBooksAsync() => await _context.Books.AsNoTracking().Where(u => u.Status == BookStatus.Unprocessed).ToArrayAsync();
        public async Task UpdateBooksStatus(Book[] booksForUpdate)
        {
            var books = await _context.Books.ToArrayAsync();
            foreach (var book in booksForUpdate)
                books.First(u => u.Id == book.Id).Status = book.Status;
            await _context.SaveChangesAsync();
        }
    }
}
