
using TestConsoleApplication.Common;
using TestConsoleApplication.Services.FileControl.Models;

namespace TestConsoleApplication.Services.FileControl
{
    public interface IFilesController
    {
        Task EnsureFilesSystemCreatedAsync();
        Task<bool> IsNeedToExport();
        Task<TCResult<List<ImportBook>>> GetBooksToImport();
    }
}