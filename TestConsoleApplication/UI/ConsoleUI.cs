using TestConsoleApplication.Common;

namespace TestConsoleApplication.UI
{
    public class ConsoleUI : IUI
    {
        private static ConsoleUI _instance;
        private ConsoleUI() { }
        public static IUI GetInstance()
        {
            _instance ??= new();
            return _instance;
        }
        public string AskString(string message)
        {
            ShowMessage(message);
            var input = Console.ReadLine();

            if (string.IsNullOrEmpty(input))
                return null;

            return input;
        }
        public int? AskInt(string message)
        {
            var input = AskString(message);

            int numericResult;

            while (!int.TryParse(input, out numericResult))
            {
                if (string.IsNullOrEmpty(input))
                    return null;

                ShowError(ExceptionsMessages.IncorrectDataType);
                input = Console.ReadLine();
            }

            return numericResult;
        }
        public TResult AskNecessaryValue<TResult>(string message, Func<string, TResult> askMethod)
        {
            TResult result;
            bool isDataRecived;

            do
            {
                result = askMethod.Invoke(message);
                isDataRecived = result != null;
            }
            while (!isDataRecived);

            return result;
        }

        public bool AskForBool(string message)
        {
            ShowMessage(message);
            ConsoleKey input;
            do
            {
                ShowMessage("Y/N?");
                input = Console.ReadKey().Key;

            } while (input != ConsoleKey.Y && input != ConsoleKey.N);

            if (input == ConsoleKey.Y)
                return true;
            return false;
        }
        public void ShowProgressMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(message);
        }
        public void ShowWarning(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(message);
        }
        public void ShowError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
        }
        public void ShowMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(message);
        }
    }
}
