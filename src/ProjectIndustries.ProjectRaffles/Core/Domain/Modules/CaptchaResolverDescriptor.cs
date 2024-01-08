namespace ProjectIndustries.ProjectRaffles.Core.Domain.Modules
{
  public class CaptchaResolverDescriptor
  {
    public CaptchaResolverDescriptor(string name)
    {
      Name = name;
    }

    public string Name { get; }
    public bool IsActive { get; internal set; }
  }
}