using System;
using System.Collections.Generic;
using System.Linq;
using AngleSharp.Html.Dom;

namespace ProjectIndustries.ProjectRaffles.Core.Services.FormParsers.ViralSweep.FieldFactories
{
  public static class HtmlElementFactory
  {
    public static KeyValuePair<string, object>[] GetOptionsAsObjValue(this IHtmlSelectElement self) =>
      self.GetOptions<object>(_ => _);

    public static KeyValuePair<string, string>[] GetOptionsWithStrValue(this IHtmlSelectElement self) =>
      self.GetOptions(_ => _);

    private static KeyValuePair<string, TValue>[] GetOptions<TValue>(this IHtmlSelectElement self,
      Func<string, TValue> converter) => self.Options
      .Where(s => !s.HasAttribute("invalid"))
      .Select(s => new KeyValuePair<string, TValue>(s.Text, converter(s.Value)))
      .ToArray();
  }
}