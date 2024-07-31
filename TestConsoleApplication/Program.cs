// See https://aka.ms/new-console-template for more information
using TestConsoleApplication;
using TestConsoleApplication.Services.Repository.Models;
using TestConsoleApplication.UI;

var app = await Application.GetApplication(ConsoleUI.GetInstance());


static class Nword
{
    public static IEnumerable<Book> GetBooksWithoutKeyword(int countOfBooks)
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