namespace ProjectIndustries.ProjectRaffles.Core.Services.Captchas
{
  public readonly struct CaptchaResult
  {
    public readonly bool Success;
    public readonly string Response;

    public CaptchaResult(bool success, string response)
    {
      Success = success;
      Response = response;
    }
  }
}