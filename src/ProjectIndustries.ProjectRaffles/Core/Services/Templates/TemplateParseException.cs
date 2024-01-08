using System;

namespace ProjectIndustries.ProjectRaffles.Core.Services.Templates
{
  public class TemplateParseException : InvalidOperationException
  {
    public TemplateParseException(string message)
      : base(message)
    {
    }
  }
}