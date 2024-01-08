using System;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using ProjectIndustries.ProjectRaffles.Core.ViewModels;
using ProjectIndustries.ProjectRaffles.WpfUI.Controls;
using ReactiveUI;

namespace ProjectIndustries.ProjectRaffles.WpfUI.Views
{
  /// <summary>
  /// Interaction logic for NavigationMenuView.xaml
  /// </summary>
  public partial class NavigationMenuView
    : ReactiveUserControl<NavigationMenuViewModel>
  {
    public NavigationMenuView()
    {
      InitializeComponent();
      this.WhenAnyValue(_ => _.ViewModel)
        .Subscribe(vm =>
        {
          DataContext = vm;
          NavContainer.Children.Clear();

          if (vm == null)
          {
            return;
          }

          bool firstChildSkipped = false;
          var buttons = vm.NavigationButtons.Select(b =>
          {
            var btn = new NavButton
            {
              Command = b.Command,
              RegularIconSrc = b.RegularIconSrc,
              ActiveIconSrc = b.ActiveIconSrc
            };

            var binding = new Binding(nameof(NavigationButtonItemViewModel.IsActivated))
            {
              Source = b
            };

            BindingOperations.SetBinding(btn, NavButton.IsActiveProperty, binding);

            return btn;
          });

          foreach (var item in buttons)
          {
            NavContainer.Children.Add(item);
            if (!firstChildSkipped)
            {
              firstChildSkipped = true;
              continue;
            }

            item.Margin = new Thickness(0, 12, 0, 0);
          }
        });
    }
  }
}