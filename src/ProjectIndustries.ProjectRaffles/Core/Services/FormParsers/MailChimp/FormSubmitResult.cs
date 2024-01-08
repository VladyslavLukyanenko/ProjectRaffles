namespace ProjectIndustries.ProjectRaffles.Core.Services.FormParsers.MailChimp
{
  public class FormSubmitResult
  {
    private FormSubmitResult(bool isSuccess, string errorMessage)
    {
      IsSuccess = isSuccess;
      ErrorMessage = errorMessage;
    }

    public static FormSubmitResult Successful() => new FormSubmitResult(true, null);
    public static FormSubmitResult Failed(string message) => new FormSubmitResult(false, message);

    public bool IsSuccess { get; }
    public string ErrorMessage { get; }
  }
}