namespace ProjectIndustries.ProjectRaffles.Core.Services.Templates
{
  public class TemplateLiteral : TemplateToken
  {
    public TemplateLiteral(string value)
    {
      Value = value;
    }

    public override TemplateTokenType TokenType => TemplateTokenType.Literal;
    public string Value { get; }

    public override string ToString()
    {
      return $"'{Value}'";
    }
  }
}