using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using ReactiveUI;

namespace ProjectIndustries.ProjectRaffles.WpfUI.Views
{
  public partial class AddressPickerView : IDisposable
  {
    public AddressPickerView()
    {
      InitializeComponent();
      this.WhenAnyValue(_ => _.ViewModel).BindTo(this, _ => _.DataContext);
      var accessToken = "pk.eyJ1IjoiZXhhbXBsZXMiLCJhIjoiY2p0MG01MXRqMW45cjQzb2R6b2ptc3J4MSJ9.zA2W0IkI0c6KaAhJfk9bWg";
      Map.AccessToken = accessToken;

      this.WhenActivated(d =>
      {
        Observable.FromEventPattern(
            h => Map.CenterChanged += h,
            h => Map.CenterChanged -= h)
          .Subscribe(_ => ViewModel.Center = Map.Center)
          .DisposeWith(d);
        Observable.FromEventPattern(
            h => Map.RadiusKmChanged += h,
            h => Map.RadiusKmChanged -= h)
          .Subscribe(_ => ViewModel.Radius = Map.RadiusKm)
          .DisposeWith(d);
      });
    }

    public void Dispose()
    {
      Map?.Dispose();
    }
  }
}