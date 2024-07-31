namespace TestConsoleApplicationTests.Environment.UI.TextAnalyzer
{
    public class NotRequireStopTextAnalyzerUI : TextAnalyzerUI
    {
        public override bool AskForBool(string message) => false;
    }
}
