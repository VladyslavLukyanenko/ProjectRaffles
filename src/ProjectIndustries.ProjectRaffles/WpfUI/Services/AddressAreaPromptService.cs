using System;
using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Services.Spatial;
using ProjectIndustries.ProjectRaffles.Core.ViewModels;
using ProjectIndustries.ProjectRaffles.WpfUI.Views;
using Splat;

namespace ProjectIndustries.ProjectRaffles.WpfUI.Services
{
  public class AddressAreaPromptService : IAddressAreaPromptService
  {
    public async Task<AddressArea> PickAddressAreaAsync(CancellationToken ct = default)
    {
      var tcs = new TaskCompletionSource<AddressArea>();
      var vm = Locator.Current.GetService<AddressPickerViewModel>();

      using var window = new AddressPickerView
      {
        ViewModel = vm
      };

      AddressArea area = null;
      vm.OkCommand.Subscribe(__ =>
      {
        area = new AddressArea(vm.Radius, vm.Center);
        window.Close();
      });

      window.Closing += (sender, args) => tcs.SetResult(area);
      window.ShowDialog();

      return await tcs.Task;
    }
  }
}