using TestConsoleApplication.Services.Logger;

namespace TestConsoleApplication.Services.Repository.Models
{
    public class AnalyzeLog
    {
        public int Id { get; set; }
        public AnalyzeStatus Status { get; set; }
        public string Message { get; set; }
        public int BookId { get; set; }
    }
}
