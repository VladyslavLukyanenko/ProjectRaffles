using System;
using AngleSharp.Dom;

namespace ProjectIndustries.ProjectRaffles.Core.Services.FormParsers.MailChimp
{
  // ReSharper disable once InconsistentNaming
  internal static class IElementExtensions
  {
    public static string SanitizeQuerySelector(this string rawSelector) => rawSelector
      .Replace("[", "\\[")
      .Replace("]", "\\]");

    public static IElement FindLabelFor(this IElement self, string elementId) =>
      self.QuerySelector($"[for={elementId.SanitizeQuerySelector()}]");

    public static string GetInputValue(this IElement self) => self.Attributes["value"]?.Value;
    public static string GetInputName(this IElement self) => self.Attributes["name"]?.Value;
    public static bool IsRequired(this IElement self) => self.ClassList.Contains("required");

    public static IElement GetParent(this IElement self, string tagName) =>
      self.GetParent(_ => _.TagName.Equals(tagName, StringComparison.InvariantCultureIgnoreCase));

    public static IElement GetParent(this IElement self, Func<IElement, bool> predicate)
    {
      IElement parentElement = self;
      while (true)
      {
        if (parentElement == null)
        {
          break;
        }

        if (predicate(parentElement))
        {
          break;
        }

        parentElement = parentElement.ParentElement;
      }

      return parentElement;
    }
  }
}