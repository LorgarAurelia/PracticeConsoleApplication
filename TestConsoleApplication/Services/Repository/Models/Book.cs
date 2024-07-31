using System.ComponentModel.DataAnnotations.Schema;

namespace TestConsoleApplication.Services.Repository.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        [Column(TypeName = "text")]
        public string Text { get; set; }
        public BookStatus Status { get; set; }
    }

    public enum BookStatus
    {
        Processed,
        Unprocessed
    }
}
