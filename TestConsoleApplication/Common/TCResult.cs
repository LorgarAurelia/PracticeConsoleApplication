using TestConsoleApplication.UI;

namespace TestConsoleApplication.Common
{
    public class TCResult<TContent>
    {
        public TContent? Content { get; set; }
        public string Message { get; set; }
        public string NETException { get; set; }
        public bool IsSuccess { get; set; }
        public ExitStatus ExitStatus { get; set; }

        private TCResult() { }

        public static TCResult<TContent?> GetError( ExitStatus exitStatus, TContent? content = default, string message = "", string netExceptionMessage = "") => new()
        {
            Content = content,
            Message = message,
            NETException = netExceptionMessage,
            ExitStatus = exitStatus,
            IsSuccess = false
        };
        public static TCResult<TContent> GetSuccessWithoutExit(TContent content)
        {
            var result = GetSuccess(content);
            result.ExitStatus = ExitStatus.NotRequired;
            result.Message = DefaultMessages.SuccessMesage;
            return result;
        }
        public static TCResult<TContent> GetSuccessWithExit(TContent content, ExitStatus exitStatus, string message = DefaultMessages.SuccessMesage)
        {
            var result = GetSuccess(content);
            result.ExitStatus = exitStatus;
            result.Message = message;
            return result;
        }
        private static TCResult<TContent> GetSuccess(TContent content) => new()
        {
            Content = content,
            NETException = string.Empty,
            IsSuccess = true
        };
    }
}
