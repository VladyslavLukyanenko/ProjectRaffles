using System;
using System.Reflection;

namespace ProjectIndustries.ProjectRaffles.Core
{
  public class AppConstants
  {
    public static Version CurrentAppVersion { get; } = (Assembly.GetEntryAssembly() ?? Assembly.GetCallingAssembly())
      .GetName()
      .Version;
    
    public const string ProductName = "Project Raffles";
    public static string ProductFullName { get; } = ProductName + " v" + CurrentAppVersion;
  }
}