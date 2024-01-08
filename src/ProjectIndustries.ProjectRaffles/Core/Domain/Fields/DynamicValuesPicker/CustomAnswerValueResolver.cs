using System;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Modules;
using Splat;

namespace ProjectIndustries.ProjectRaffles.Core.Domain.Fields.DynamicValuesPicker
{
  public class CustomAnswerValueResolver : IDynamicValueResolver, IValueChangePostProcessable
  {
    private string _answerText;

    public CustomAnswerValueResolver()
    {
      ResolveValue = context => Task.FromResult(_answerText);
    }

    public string Name => "Custom Text";
    public Func<IRaffleExecutionContext, Task<string>> ResolveValue { get; }
    public async Task PostProcessAsync(IReadonlyDependencyResolver dependencyResolver)
    {
      var answer = new TextField(displayName: "Enter text");
      var presenter = dependencyResolver.GetService<IValueResolverConfigurationPresenter>();
      await presenter.ShowConfigurationWindowAsync("Custom Text", answer);

      _answerText = answer.Value;
    }
  }
}