using Microsoft.EntityFrameworkCore;
using TestConsoleApplication.Services.Repository.Models;

namespace TestConsoleApplication.Services.Repository
{
    public class BookContext : DbContext
    {
        public DbSet<Book> Books { get; set; }
        public DbSet<AnalyzeLog> Logs { get; set; }

        private readonly string _connectionString;

        public BookContext(string connectionString)
        {
            _connectionString = connectionString;
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder.UseNpgsql(_connectionString);
    }
}
