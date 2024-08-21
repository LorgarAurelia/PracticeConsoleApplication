using System.Runtime.InteropServices;
using TestConsoleApplication.Common;
using TestConsoleApplication.Services.Analize;
using TestConsoleApplication.Services.Logger;
using TestConsoleApplication.Services.Repository.Models;
using TestConsoleApplication.UI;
using TestConsoleApplicationTests.Environment;
using TestConsoleApplicationTests.Environment.UI.TextAnalyzer;

namespace TestConsoleApplicationTests.TextAnalyzerTests
{
    [TestClass]
    public class TextAnalyzerTests
    {
        private readonly AnalyzerSettings _settings = new() { ClusterSize = 10, Keyword = "keyword" };
        private readonly int _countOfBooks = 50;

        [TestMethod]
        public void TextWithoutKeyword_CorrectProcessing()
        {
            var books = GetBooksWithoutKeyword(_countOfBooks).ToArray();
            var textAnalyzer = GetTextAnalyzerWhichNeverStop(out var ui, out var logger);

            var result = textAnalyzer.FindKeyWord(books, _settings);

            AssertCorrectExpectedResult(result, ui, logger);
        }
        [TestMethod]
        public void TextWithOneKeyword_ContinueProcessingAfterAsk()
        {
            var books = GetBooksWithoutKeyword(_countOfBooks).ToArray();
            books[26].Text = _settings.Keyword;
            var textAnalyzer = GetTextAnalyzerWhichNeverStop(out var ui, out var logger);

            var result = textAnalyzer.FindKeyWord(books, _settings);

            AssertCorrectExpectedResult(result, ui, logger);
        }
        [TestMethod]
        public void TextWithKeyword_StopedAfterAsk()
        {
            var books = GetBooksWithoutKeyword(_countOfBooks).ToArray();
            books[26].Text += _settings.Keyword;
            var targetBookId = books[26].Id;
            var textAnalyzer = GetTextAnalyzerWhichWillStopOnRequest(out var ui, out var logger);

            var result = textAnalyzer.FindKeyWord(books, _settings);

            AssertStopExpectedResult(result, ui, logger);

            var targetBook = result.Content.FirstOrDefault(u => u.Id == targetBookId);
            Assert.AreEqual(BookStatus.Unprocessed, targetBook.Status);

            var targetBookLog = logger.Logs.FirstOrDefault(u => u.BookId == targetBookId);
            Assert.IsNotNull(targetBookLog);
            Assert.AreEqual(AnalyzeStatus.Stoped, targetBookLog.Status);
        }
        [TestMethod]
        public void TextWithKeyword_SeveralBooksWithKeyword_StopedAfterAsk()
        {
            var books = GetBooksWithoutKeyword(_countOfBooks).ToArray();
            books[26].Text += _settings.Keyword;
            books[4].Text += _settings.Keyword;
            int[] targetBooksIds = [books[26].Id, books[4].Id];
            var textAnalyzer = GetTextAnalyzerWhichWillStopOnRequest(out var ui, out var logger);

            var result = textAnalyzer.FindKeyWord(books, _settings);

            AssertStopExpectedResult(result, ui, logger);

            var targetBooksStatuses = result.Content.Where(u => targetBooksIds.Contains(u.Id)).Select(u => u.Status).ToArray();
            foreach (var status in targetBooksStatuses)
                Assert.AreEqual(BookStatus.Unprocessed, status);
            
            var targetBooksLogs = logger.Logs.Where(u => u.Status == AnalyzeStatus.Stoped).ToArray();
            Assert.AreEqual(1, targetBooksLogs.Length);
        }
        private void AssertStopExpectedResult(TCResult<Book[]?> result, RequireStopTextAnalyzerUI ui, LoggerForTests logger)
        {
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(0, ui.ErrorOutputs.Count);
            Assert.AreEqual(1, ui.WarningOutputs.Count);

            var unprocessedBooksStateCount = result.Content.Where(u => u.Status == BookStatus.Unprocessed).Count();
            Assert.AreNotEqual(0, unprocessedBooksStateCount);
            
            var errorLogsCount = logger.Logs.Where(u => u.Status == AnalyzeStatus.Stoped).Count();
            Assert.AreNotEqual(0, errorLogsCount);
        }
        private void AssertCorrectExpectedResult(TCResult<Book[]?> result, NotRequireStopTextAnalyzerUI ui, LoggerForTests logger)
        {
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(0, ui.ErrorOutputs.Count);
            Assert.AreNotEqual(0, ui.ProgressOutputs.Count);

            for (var i = 0; i < _countOfBooks; i++)
            {
                Assert.AreNotEqual(BookStatus.Unprocessed, result.Content[i].Status);
                var bookLog = logger.Logs.FirstOrDefault(u => u.BookId == result.Content[i].Id);
                Assert.IsNotNull(bookLog);
                Assert.AreEqual(AnalyzeStatus.Finished, bookLog.Status);
            }
        }
        private ITextAnalyzer GetTextAnalyzerWhichNeverStop(out NotRequireStopTextAnalyzerUI ui, out LoggerForTests logger) => GetTextAnalyzer(ui = new(), logger = new());

        private ITextAnalyzer GetTextAnalyzerWhichWillStopOnRequest(out RequireStopTextAnalyzerUI ui, out LoggerForTests logger) => GetTextAnalyzer(ui = new(), logger = new());

        private ITextAnalyzer GetTextAnalyzer(IUI ui, ITCLogger logger) => new TextAnalyzer(logger, ui);

        private static IEnumerable<Book> GetBooksWithoutKeyword(int countOfBooks)
        {
            for (int i = 1; i <= countOfBooks; i++)
            {
                yield return new Book()
                {
                    Id = i,
                    Status = BookStatus.Unprocessed,
                    Title = $"Book number: {i}",
                    Text = $"Text of book with number: {i}"
                };
            }
        }
    }
}