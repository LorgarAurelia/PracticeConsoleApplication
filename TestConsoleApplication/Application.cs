using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TestConsoleApplication.Common;
using TestConsoleApplication.Services.FileControl;
using TestConsoleApplication.Services.Repository;
using TestConsoleApplication.Services.Repository.Models;
using TestConsoleApplication.UI;

namespace TestConsoleApplication
{
    public class Application
    {
        public IBookCatalog BookCatalog { get => _serviceProvider.GetRequiredService<IBookCatalog>(); }
        public IFilesController FilesController { get => _serviceProvider.GetRequiredService<IFilesController>(); }

        private IUI _userInterface;
        private ServiceProvider _serviceProvider;
        private IMapper _mapper => _serviceProvider.GetRequiredService<IMapper>();
        private static Application _appInstance;
        private Application() { }

        public static async Task<Application> GetApplication(IUI userInterface)
        {
            if (_appInstance == null)
            {
                var app = new Application();

                app._userInterface = userInterface;
                var builder = Host.CreateApplicationBuilder();
                var settings = builder.BuildAppConfiguration(userInterface);
                builder.InjectDependency(settings);
                builder.Build();

                app._serviceProvider = builder.Services.BuildServiceProvider();
                await app.FilesController.EnsureFilesSystemCreatedAsync();

                _appInstance = app;
            }
            return _appInstance;
        }

        public async Task Start()
        {
            if (await FilesController.IsNeedToExport())
            {
                var exportBooksResult = await FilesController.GetBooksToImport();
                if (exportBooksResult.IsSuccess)
                {
                    var books = _mapper.Map<Book[]>(exportBooksResult.Content);
                    await BookCatalog.SaveBooksAsync(books);
                }
                else
                    HandleException(exportBooksResult);
            }

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
