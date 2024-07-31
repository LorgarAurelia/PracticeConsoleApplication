using TestConsoleApplication.UI;

namespace TestConsoleApplicationTests.Environment.UI
{
    public abstract class BaseTestUI : IUI
    {
        public List<string> ProgressOutputs { get; set; } = new();
        public List<string> WarningOutputs { get; set; } = new();
        public List<string> ErrorOutputs { get; set; } = new();
        public List<string> MessagesOutputs { get; set; } = new();

        public abstract bool AskForBool(string message);
        public abstract int? AskInt(string message);
        public abstract string AskString(string message);

        public virtual TResult AskNecessaryValue<TResult>(string message, Func<string, TResult> askMethod) => askMethod.Invoke(message);

        public void ShowError(string message) => ErrorOutputs.Add(message);

        public void ShowProgressMessage(string message) => ProgressOutputs.Add(message);

        public void ShowWarning(string message) => WarningOutputs.Add(message);

        public void ShowMessage(string message) =>  MessagesOutputs.Add(message);
    }
}
