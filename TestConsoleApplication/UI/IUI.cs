
namespace TestConsoleApplication.UI
{
    public interface IUI
    {
        bool AskForBool(string message);
        int? AskInt(string message);
        TResult AskNecessaryValue<TResult>(string message, Func<string, TResult> askMethod);
        string AskString(string message);
        void ShowError(string message);
        void ShowProgressMessage(string message);
        void ShowWarning(string message);
        void ShowMessage(string message);
    }
}