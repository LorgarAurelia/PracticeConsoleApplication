using TestConsoleApplication.Common;
using TestConsoleApplication.Services.Repository.Models;

namespace TestConsoleApplication.Services.Analize
{
    public interface ITextAnalyzer
    {
        TCResult<Book[]> FindKeyWord(Book[] books, AnalyzerSettings searchSettings);
    }
}