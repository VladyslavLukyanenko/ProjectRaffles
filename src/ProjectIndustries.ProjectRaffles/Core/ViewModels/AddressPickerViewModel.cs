using System.Reactive;
using System.Reactive.Linq;
using ProjectIndustries.ProjectRaffles.WpfUI.MapBox.MapboxNetCore;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace ProjectIndustries.ProjectRaffles.Core.ViewModels
{
  public class AddressPickerViewModel : ViewModelBase
  {
    public AddressPickerViewModel()
    {
      var isOk = this.WhenAnyValue(_ => _.Center)
        .CombineLatest(this.WhenAnyValue(_ => _.Radius), (c, r) => c != null && r > 0);
      OkCommand = ReactiveCommand.Create(() => { }, isOk);
    }

    [Reactive] public GeoLocation Center { get; set; } = new GeoLocation(-0.2416815, 51.5285582);
    [Reactive] public double Radius { get; set; }

    public ReactiveCommand<Unit, Unit> OkCommand { get; private set; }
  }
}