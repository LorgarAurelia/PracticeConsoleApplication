using TestConsoleApplication.Common;
using TestConsoleApplication.Log;
using TestConsoleApplication.Services.Logger;
using TestConsoleApplication.Services.Repository.Models;
using TestConsoleApplication.UI;

namespace TestConsoleApplication.Services.Analize
{
    public class TextAnalyzer(ITCLogger logger, IUI userInterface) : ITextAnalyzer
    {
        private AnalyzerSettings _settings;
        private static readonly object _locker = new();
        private TextAnalyzerState _state = TextAnalyzerState.Process;
        private readonly ITCLogger _logger = logger;
        private readonly IUI _userInterface = userInterface;
        private int _totalBooks = 0;

        public TCResult<Book[]> FindKeyWord(Book[] books, AnalyzerSettings searchSettings)
        {
            _settings = searchSettings;
            _totalBooks = books.Length;

            var booksClusters = PrepareClusters(books);
            List<Thread> threads = new();//ThreadPool показал себя неэффективно с такими короткими задачами
            foreach (var cluster in booksClusters)
            {
                Thread clusterAnalyze = new(() => SearchAndNotify(cluster));
                clusterAnalyze.Start();
                threads.Add(clusterAnalyze);
            }

            foreach (var thread in threads)
                thread.Join();

            if (_state == TextAnalyzerState.Stopped)
                return TCResult<Book[]>.GetError(ExitStatus.RequiredByUser, books);
            return TCResult<Book[]>.GetSuccessWithoutExit(books);
        }


        private List<List<Book>> PrepareClusters(Book[] books)
        {
            var clustersCount = Math.Ceiling((float)(_totalBooks / _settings.ClusterSize));
            var totalCounter = 0;

            List<List<Book>> clusters = new();

            for (int clustersCounter = 0; clustersCounter < clustersCount; clustersCounter++)
            {
                List<Book> currentCluster = new();

                for (int clusterCounter = 0; clusterCounter < _settings.ClusterSize; clusterCounter++)
                {
                    if (totalCounter >= _totalBooks)
                        break;

                    currentCluster.Add(books[totalCounter]);
                    totalCounter++;
                }
                clusters.Add(currentCluster);
            }

            return clusters;
        }
        private void SearchAndNotify(IEnumerable<Book> books)
        {
            foreach (var book in books)
            {
                while (_state == TextAnalyzerState.Paused)
                    Thread.Sleep(1000);

                if (_state == TextAnalyzerState.Stopped)
                    break;

                if (book.Text.Contains(_settings.Keyword))
                {
                    lock (_locker)
                    {
                        if (_state == TextAnalyzerState.Stopped)
                            break;

                        _state = TextAnalyzerState.Paused;


                        if (_userInterface.AskForBool(DefaultMessages.AskForContinueSearch))
                        {
                            _state = TextAnalyzerState.Stopped;
                            _logger.LogBook(new() { BookId = book.Id, Status = AnalyzeStatus.Stoped, Message = DefaultMessages.StopedByUser });
                            _userInterface.ShowWarning(DefaultMessages.StopedByUser);
                            continue;
                        }

                        _state = TextAnalyzerState.Process;
                    }
                }

                _userInterface.ShowProgressMessage($"{DefaultMessages.ShowProgressWithId} {book.Id}");
                book.Status = BookStatus.Processed;
                _logger.LogBook(new() { BookId = book.Id, Status = AnalyzeStatus.Finished, Message = DefaultMessages.SuccessMesage }).Wait();

            }
        }
    }
}
