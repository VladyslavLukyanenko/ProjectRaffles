using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ProjectIndustries.ProjectRaffles.Core.ViewModels;
using ProjectIndustries.ProjectRaffles.Core.ViewModels.Accounts;

namespace ProjectIndustries.ProjectRaffles.WpfUI.Views
{
    /// <summary>
    /// Interaction logic for AccountsView.xaml
    /// </summary>
    public partial class AccountsView
        : ReactiveUserControl<AccountsViewModel>
    {
        public AccountsView()
        {
            InitializeComponent();
            this.WhenAnyValue(_ => _.ViewModel).BindTo(this, _ => _.DataContext);
        }
    }
}
