using System.Text.Json;
using TestConsoleApplication.Common;
using TestConsoleApplication.Services.FileControl.Models;

namespace TestConsoleApplication.Services.FileControl
{
    public class FilesController : IFilesController
    {
        private Settings _appSettings;
        private string _externalStoragePath;
        private FileControllerState _state = FileControllerState.JustCreated;
        public FilesController(Settings appSettings) => _appSettings = appSettings;

        public async Task EnsureFilesSystemCreatedAsync()
        {
            if (_state == FileControllerState.Initialized)
                return;

            var readSettingsTask = ReadAppSettingsAsync();
            var directoryEnsuringTask = EnsureDirectoryCreatedAsync();
            await Task.WhenAll(readSettingsTask, directoryEnsuringTask);

            var settingsFromJson = JsonSerializer.Deserialize<Settings>(readSettingsTask.Result);

            var updateSettingsTask = UpdateAppSettingIfRequiredAsync(settingsFromJson);
            var ImportSorageInsuringTask = EnsureImportStorageCreatedAsync();
            await Task.WhenAll(updateSettingsTask, ImportSorageInsuringTask);

            _state = FileControllerState.Initialized;
        }
        public async Task<bool> IsNeedToExport()
        {
            InitializeRequire();

            using var reader = new StreamReader(_externalStoragePath);
            var result = await reader.ReadLineAsync();

            if(string.IsNullOrEmpty(result))
                return false;
            return true;
        }

        public async Task<TCResult<List<ImportBook>>> GetBooksToImport()
        {
            InitializeRequire();

            using var reader = new StreamReader(_externalStoragePath);
            var rawBooks = await reader.ReadToEndAsync();
            var formattedBooks = new List<ImportBook>();
            try
            {
                var uvalidatedBooks = JsonSerializer.Deserialize<ImportBook[]>(rawBooks);

                foreach (var book in formattedBooks)
                {
                    if (!string.IsNullOrEmpty(book.Text))
                        formattedBooks.Add(book);
                }
            }
            catch (JsonException ex)
            {
                return TCResult<List<ImportBook>>.GetError(ExitStatus.FileSystemException, 
                    message: ExceptionsMessages.CorruptedExportStorage, netExceptionMessage: ex.Message);
            }
            return TCResult<List<ImportBook>>.GetSuccessWithoutExit(formattedBooks);
        }
        private async Task<string> ReadAppSettingsAsync()
        {
            using var reader = new StreamReader("appSettings.json");
            var result = await reader.ReadToEndAsync();
            reader.Close();
            return result;
        }
        private async Task EnsureDirectoryCreatedAsync()
        {
            if (!Directory.Exists(_appSettings.FilesRoot))
                await Task.Run(() => Directory.CreateDirectory(_appSettings.FilesRoot));
        }
        private async Task UpdateAppSettingIfRequiredAsync(Settings appSettingsFromJson)
        {
            if (string.IsNullOrEmpty(appSettingsFromJson.DBConnectionString) || string.IsNullOrEmpty(appSettingsFromJson.FilesRoot))
            {
                using var writer = new StreamWriter("appSettings.json", false);
                var json = JsonSerializer.Serialize(_appSettings);
                await writer.WriteAsync(json);
                writer.Close();
            }
        }
        private async Task EnsureImportStorageCreatedAsync()
        {
            _externalStoragePath = $"{_appSettings.FilesRoot}\\ImportStorage.json";
            if (!File.Exists(_externalStoragePath))
                await Task.Run(() => File.Create(_externalStoragePath));
        }
        private void InitializeRequire()
        {
            if (_state == FileControllerState.JustCreated)
                throw new Exception(ExceptionsMessages.FileControllerDosentInitialized);
        }
    }
}
