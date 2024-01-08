using System.Collections.Generic;
using AngleSharp.Html.Dom;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;

namespace ProjectIndustries.ProjectRaffles.Core.Services.FormParsers.ViralSweep.FieldFactories
{
  public interface IFieldFromHtmlFieldFactory
  {
    bool CanHandle(IEnumerable<string> classes);
    IEnumerable<Field> Create(IHtmlElement element);
  }
}