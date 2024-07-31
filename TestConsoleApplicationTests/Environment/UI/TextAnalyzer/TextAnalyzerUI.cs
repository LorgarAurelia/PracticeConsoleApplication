namespace TestConsoleApplicationTests.Environment.UI.TextAnalyzer
{
    public abstract class TextAnalyzerUI : BaseTestUI
    {
        protected Random _random = new();
        public override int? AskInt(string message) => _random.Next();
        public override string AskString(string message)
        {
            throw new NotImplementedException();
        }
    }
}
