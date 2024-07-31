using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TestConsoleApplication.Services.Analize;
using TestConsoleApplication.Services.FileControl;
using TestConsoleApplication.Services.Logger;
using TestConsoleApplication.Services.Mapper;
using TestConsoleApplication.Services.Repository;
using TestConsoleApplication.UI;

namespace TestConsoleApplication.Common
{
    public static class HostApplicationBuilderExtension
    {
        public static void InjectDependency(this HostApplicationBuilder builder, Settings settings)
        {
            builder.Services
            .AddSingleton<BookContext>(u => new(settings.DBConnectionString))
            .AddTransient<IBookCatalog, BookCatalog>()
            .AddSingleton<IFilesController, FilesController>( u => new(settings))
            .AddTransient<ITCLogger, Logger>()
            .AddSingleton<IUI, ConsoleUI>()
            .AddTransient<ITextAnalyzer, TextAnalyzer>()
            .AddAutoMapper(typeof(BookProfile));
        }
        public static Settings BuildAppConfiguration(this HostApplicationBuilder builder, IUI userInterface)
        {
            builder.Configuration.AddJsonFile("appSettings.json", true, true);
            builder.Configuration.AddJsonFile("textAnalyzeSetting.json", false, true);

            var appSettings = builder.Configuration.Get<Settings>();
            VerifyAnalyzeSettings(builder.Configuration.Get<AnalyzerSettings>(), userInterface);

            appSettings ??= new Settings();

            if (string.IsNullOrEmpty(appSettings.DBConnectionString))
                appSettings.DBConnectionString = userInterface.AskNecessaryValue<string>(DefaultMessages.AskForConnectionString, userInterface.AskString);

            if (string.IsNullOrEmpty(appSettings.FilesRoot))
                appSettings.FilesRoot =
                $"{builder.Configuration.GetValue<string>("HOMEDRIVE")}" +
                $"{builder.Configuration.GetValue<string>("HOMEPATH")}" +
                $"\\{AppDomain.CurrentDomain.FriendlyName}";

            return appSettings;
        }
        private static void VerifyAnalyzeSettings(AnalyzerSettings settings, IUI userInterface)
        {
            if (settings == null || string.IsNullOrEmpty(settings.Keyword) || settings.ClusterSize == 0)
                if (settings == null)
                {
                    userInterface.ShowError(ExceptionsMessages.TextAnalyzerMissing);
                    Environment.Exit((int)ExitStatus.StartupException);
                }
                    
                else if (settings.ClusterSize == 0) 
                {
                    userInterface.ShowError(ExceptionsMessages.ClusterSyzeRule);
                    Environment.Exit((int)ExitStatus.StartupException);
                }
                   
                else 
                {
                    userInterface.ShowError(ExceptionsMessages.KeywordMissing);
                    Environment.Exit((int)ExitStatus.StartupException);
                }

        }
    }
}
