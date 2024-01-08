using System;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using DynamicData;
using ProjectIndustries.ProjectRaffles.Core.Domain;
using ProjectIndustries.ProjectRaffles.Core.Domain.Emails;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;
using ProjectIndustries.ProjectRaffles.Core.Services;
using ProjectIndustries.ProjectRaffles.Core.Services.Emails;

namespace ProjectIndustries.ProjectRaffles.WpfUI.Services.Fields
{
  public class EmailPickerFieldControlFactory : SingleFieldControlFactoryBase<EmailPickerField>
  {
    private readonly ReadOnlyObservableCollection<Email> _emails;

    public EmailPickerFieldControlFactory(IEmailRepository emailRepository)
    {
      emailRepository.Items.Connect()
        .Bind(out _emails)
        .DisposeMany()
        .Subscribe();
    }

    protected override Control CreateEditorControl(EmailPickerField field)
    {
      var itemsSource = new Binding("")
      {
        Source = _emails,
      };

      var selectedValue = new Binding(nameof(Field.Value))
      {
        Source = field
      };

      var control = new ComboBox
      {
        DisplayMemberPath = nameof(Email.Value)
      };

      control.SetBinding(ItemsControl.ItemsSourceProperty, itemsSource);
      control.SetBinding(Selector.SelectedItemProperty, selectedValue);

      return control;
    }
  }
}