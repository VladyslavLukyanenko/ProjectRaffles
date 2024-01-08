using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Win32;
using ProjectIndustries.ProjectRaffles.Core.Services;

namespace ProjectIndustries.ProjectRaffles.WpfUI.Services
{
  public class WpfDialogService : IDialogService
  {
    public Task<string> PickOpenFileAsync(string title, params string[] validExtensions)
    {
      var dialog = new OpenFileDialog
      {
        Title = title
      };

      if (dialog.ShowDialog() == true)
      {
        return Task.FromResult(dialog.FileName);
      }

      return Task.FromResult((string) null);
    }

    public Task<string> PickSaveFileAsync(string title, string ext, string defaultFileName = null)
    {
      var dialog = new SaveFileDialog
      {
        Title = title,
        DefaultExt = ext,
        FileName = defaultFileName!
      };

      if (dialog.ShowDialog() == true)
      {
        return Task.FromResult(dialog.FileName);
      }

      return Task.FromResult((string) null);
    }

    public void ShowDirectory(string path)
    {
      Process.Start(new ProcessStartInfo
      {
        FileName = path,
        UseShellExecute = true,
        Verb = "open"
      });
    }
  }
}