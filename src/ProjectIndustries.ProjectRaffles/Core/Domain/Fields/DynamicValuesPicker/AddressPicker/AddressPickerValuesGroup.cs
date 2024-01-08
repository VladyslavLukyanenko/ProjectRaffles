using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Services.Spatial;
using Splat;

namespace ProjectIndustries.ProjectRaffles.Core.Domain.Fields.DynamicValuesPicker.AddressPicker
{
  public class AddressPickerValuesGroup : IDynamicValuesGroup, IAddressPartResolutionProvider,
    ICloneableDynamicValuesGroup
  {
    private readonly SemaphoreSlim _gates = new SemaphoreSlim(1, 1);
    private AddressArea _area;
    private ReversedLocation _reversedLocation;
    private IAddressReverseService _reverseService;

    private AddressPickerValuesGroup(Func<AddressPickerValuesGroup, IDynamicValueResolver[]> resolversFactory)
    {
      ValueResolvers = resolversFactory(this);
    }

    public static AddressPickerValuesGroup CreateOptimized() =>
      new AddressPickerValuesGroup(g => CreateOptimizedResolvers(g).ToArray());

    public static AddressPickerValuesGroup CreateFull() => new AddressPickerValuesGroup(CreateWithAllResolvers);

    public void TryCopyTo(IDynamicValuesGroup targetGroup)
    {
      var target = (AddressPickerValuesGroup) targetGroup;
      target._area = _area;
      target._reverseService = _reverseService;
    }

    public string Group { get; } = "Address Picker";
    public IReadOnlyList<IDynamicValueResolver> ValueResolvers { get; private set; }

    public async Task<ReversedLocation> GetAddressAsync()
    {
      const int retryCount = 10;
      try
      {
        await _gates.WaitAsync(CancellationToken.None);
        var attempt = 1;
        do
        {
          attempt++;
          if (_reversedLocation == null)
          {
            var loc = _area.GetNextRandomPointInArea();

            _reversedLocation = await _reverseService.ReverseAddressAsync(loc);
          }

          if (ValueResolvers.OfType<AddressPartPickerBase>().All(_ => _.IsValid(_reversedLocation.Address)))
          {
            return _reversedLocation;
          }
          else
          {
            _reversedLocation = null;
          }
        } while (attempt < retryCount);
      }
      finally
      {
        _gates.Release();
      }

      throw new InvalidOperationException("Can't pick address. Please increase area and try again.");
    }

    public async Task InitializeAreaAsync(IReadonlyDependencyResolver dependencyResolver)
    {
      try
      {
        await _gates.WaitAsync(CancellationToken.None);
        if (_area != null)
        {
          return;
        }

        var addressPicker = dependencyResolver.GetService<IAddressAreaPromptService>();
        _reverseService = dependencyResolver.GetService<IAddressReverseService>();

        _area = await addressPicker.PickAddressAreaAsync();
      }
      finally
      {
        _gates.Release();
      }
    }

    private static List<IDynamicValueResolver> CreateOptimizedResolvers(AddressPickerValuesGroup group)
    {
      return new List<IDynamicValueResolver>
      {
        new RandomAddressLine1ValueResolver(group),
        new RandomAddressLine2ValueResolver(group),
        // new RandomCityValueResolver(this),
        new RandomStateValueResolver(group),
        new RandomCountryCodeValueResolver(group),
        new RandomPostCodeValueResolver(group),
      };
    }

    private static IDynamicValueResolver[] CreateWithAllResolvers(AddressPickerValuesGroup group)
    {
      var all = CreateOptimizedResolvers(group);
      all.Add(new RandomCityValueResolver(group));

      return all.ToArray();
    }

    public IDynamicValuesGroup CreateClone()
    {
      var copy = (AddressPickerValuesGroup) MemberwiseClone();
      copy._reversedLocation = null;
      copy.ValueResolvers = ValueResolvers.Cast<AddressPartPickerBase>()
        .Select(_ => _.CreateClone(copy))
        .ToArray();

      return copy;
    }
  }
}