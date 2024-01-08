using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields.DynamicValuesPicker;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields.Validation;
using ProjectIndustries.ProjectRaffles.Core.Modules;
using Splat;

namespace ProjectIndustries.ProjectRaffles.Core.Domain.Fields
{
  public class DynamicValuesPickerField : Field<string>, IRequiresPreInitialization
  {
    private IReadOnlyList<IDynamicValuesGroup> _groups = Array.Empty<IDynamicValuesGroup>();
    private IReadonlyDependencyResolver _dependencyResolver;
    private bool _isInitializing;

    private readonly BehaviorSubject<IDynamicValueResolver> _selectedResolver =
      new BehaviorSubject<IDynamicValueResolver>(null);

    public DynamicValuesPickerField()
    {
    }

    public DynamicValuesPickerField(params IDynamicValuesGroup[] groups)
      : this(null, null, false, null, groups)
    {
    }

    public DynamicValuesPickerField(string displayName)
      : this(null, displayName, true, groups: Pickers.All)
    {
    }

    public DynamicValuesPickerField(string displayName, params IDynamicValuesGroup[] groups)
      : this(null, displayName, true, groups: groups)
    {
    }

    public DynamicValuesPickerField(string systemName, string displayName, bool isRequired,
      Func<string, Task<bool>> validator = null, params IDynamicValuesGroup[] groups)
      : base(systemName, displayName, isRequired, validator ?? FieldValidators.AlwaysValid<string>())
    {
      _groups = groups;
      Changed = base.Changed.CombineLatest(_selectedResolver, (f, _) => f);
      _selectedResolver.OfType<IValueChangePostProcessable>()
        .Where(_ => !_isInitializing)
        .Subscribe(async r => { await r.PostProcessAsync(_dependencyResolver); });

      _isInitializing = false;
    }

    public override bool IsEmpty => _selectedResolver.Value == null;

    public override IObservable<Field> Changed { get; }

    public override string ValueId => Value;
    public override string DisplayValue => Value;

    public IReadOnlyList<IDynamicValuesGroup> Groups => _groups;

    public IDynamicValueResolver SelectedResolver
    {
      get => _selectedResolver.Value;
      set => _selectedResolver.OnNext(value);
    }

    public override Task InitializeAsync(IReadonlyDependencyResolver dependencyResolver, CancellationToken ct)
    {
      _dependencyResolver = dependencyResolver;
      return base.InitializeAsync(dependencyResolver, ct);
    }

    public override void CopyTo(Field field)
    {
      base.CopyTo(field);
      var pickerField = (DynamicValuesPickerField) field;
      pickerField._isInitializing = true;

      var parentGroup = _groups.FirstOrDefault(_ => _.ValueResolvers.Contains(SelectedResolver));
      if (parentGroup is ICloneableDynamicValuesGroup cg)
      {
        var clone = cg.CreateClone();
        pickerField._groups = new[] {clone};
        pickerField.SelectedResolver = clone.ValueResolvers.First(r => r.Name == SelectedResolver.Name);
      }
      else if (parentGroup != null && pickerField._groups.Count == _groups.Count)
      {
        var targetGroup = pickerField._groups.First(_ => _.Group == parentGroup.Group);
        parentGroup.TryCopyTo(targetGroup);
        pickerField.SelectedResolver = targetGroup.ValueResolvers.First(r => r.Name == SelectedResolver.Name);
      }
      else
      {
        pickerField.SelectedResolver = SelectedResolver;
        pickerField._groups = _groups.ToList();
      }

      pickerField._isInitializing = false;
    }

    public async Task PrepareAsync(IRaffleExecutionContext context, CancellationToken ct = default)
    {
      if (SelectedResolver == null)
      {
        return;
      }

      Value = await SelectedResolver.ResolveValue(context);
    }
  }
}