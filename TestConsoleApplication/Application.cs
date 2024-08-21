using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TestConsoleApplication.Common;
using TestConsoleApplication.Services.Analize;
using TestConsoleApplication.Services.FileControl;
using TestConsoleApplication.Services.Repository;
using TestConsoleApplication.Services.Repository.Models;
using TestConsoleApplication.UI;

namespace TestConsoleApplication
{
    public class Application
    {
        private IBookCatalog _bookCatalog => _serviceProvider.GetRequiredService<IBookCatalog>();
        private IFilesController _filesController => _serviceProvider.GetRequiredService<IFilesController>();
        private ITextAnalyzer _textAnalyzer => _serviceProvider.GetRequiredService<ITextAnalyzer>();
        private IUI _userInterface => _serviceProvider.GetRequiredService<IUI>();
        private AnalyzerSettings _analyzerSettings;

        private ServiceProvider _serviceProvider;
        private IMapper _mapper => _serviceProvider.GetRequiredService<IMapper>();
        private static Application _appInstance;
        private Application() { }

        public static async Task<Application> GetApplication()
        {
            if (_appInstance == null)
            {
                var app = new Application();

                var builder = Host.CreateApplicationBuilder();
                var settings = builder.BuildAppConfiguration();
                app._analyzerSettings = settings.AnalyzerSettings;
                builder.InjectDependency(settings.AppSettings);
                builder.Build();

                app._serviceProvider = builder.Services.BuildServiceProvider();
                await app._filesController.EnsureFilesSystemCreatedAsync();

                _appInstance = app;
            }
            return _appInstance;
        }

        public async Task Execute()
        {
            if (await _filesController.IsNeedToExport())
            {
                var exportBooksResult = await _filesController.GetBooksToImport();
                if (exportBooksResult.IsSuccess)
                {
                    var booksToDb = _mapper.Map<Book[]>(exportBooksResult.Content);
                    await _bookCatalog.SaveBooksAsync(booksToDb);
                }
                else
                    HandleException(exportBooksResult);
            }

            var books = await _bookCatalog.GetUnfinishedBooksAsync();

            var analyzeResult = _textAnalyzer.FindKeyWord(books, _analyzerSettings);

            if (analyzeResult.Content != null || analyzeResult.Content.Length > 0)
                await _bookCatalog.UpdateBooksStatus(analyzeResult.Content);

            if (analyzeResult.ExitStatus != ExitStatus.NotRequired)
                CloseApplication(analyzeResult.ExitStatus);

            if(analyzeResult.IsSuccess == false)
                HandleException(analyzeResult);
            
            CloseApplication(ExitStatus.Success);
        }

        private void CloseApplication(ExitStatus exitStatus)
        {
            _userInterface.ShowMessage(DefaultMessages.CorrectShutdown);
            Environment.Exit((int)exitStatus);
        }

        private void HandleException<TContent>(TCResult<TContent> result)
        {
            _userInterface.ShowError($"Internal application message: {result.Message}");
            _userInterface.ShowError($".NET Exception: {result.NETException}");

            if (result.ExitStatus != ExitStatus.NotRequired)
                Environment.Exit((int)result.ExitStatus);
        }
    }
}
