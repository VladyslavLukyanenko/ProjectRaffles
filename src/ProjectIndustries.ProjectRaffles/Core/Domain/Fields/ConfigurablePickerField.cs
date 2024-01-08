// using System;
// using System.Collections.Generic;
// using System.Threading;
// using System.Threading.Tasks;
// using ProjectIndustries.ProjectRaffles.Core.Domain.Fields.DynamicValuesPicker;
// using ProjectIndustries.ProjectRaffles.Core.Domain.Fields.Validation;
// using ProjectIndustries.ProjectRaffles.Core.Modules;
// using Splat;
//
// namespace ProjectIndustries.ProjectRaffles.Core.Domain.Fields
// {
//   public class ConfigurablePickerField : IDynamicValueResolver, IConfigurableValueResolver
//   {
//     private IDynamicValueResolver _selectedResolver;
//
//     public async Task ConfigureAsync(IReadonlyDependencyResolver dependencyResolver)
//     {
//       var presenter = dependencyResolver.GetService<IFieldConfigurationPresenter>();
//       var picker = new SelectField<IDynamicValueResolver>(
//         new KeyValuePair<string, IDynamicValueResolver>("Random Email", Pickers.Misc.Email),
//         new KeyValuePair<string, IDynamicValueResolver>("Random List Item", Pickers.Misc.ListItem)
//       );
//
//       await presenter.ShowConfigurationWindowAsync("Please select type of value to generate", picker);
//       IDynamicValueResolver selectedResolver = picker.Value;
//       _selectedResolver = selectedResolver;
//     }
//
//     public string Name { get; } = "Random Email/List item";
//     public Func<RaffleExecutionContext, IRaffleModule, Task<string>> ResolveValue => _selectedResolver.ResolveValue;
//   }
// }