using System.Threading.Tasks;

namespace ProjectIndustries.ProjectRaffles.Core.Services
{
  public interface IDialogService
  {
    Task<string> PickOpenFileAsync(string title, params string[] validExtensions);
    Task<string> PickSaveFileAsync(string title, string ext, string defaultFileName = null);
    void ShowDirectory(string path);
  }
}