using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Domain.Modules;
using ProjectIndustries.ProjectRaffles.Core.Events;
using ProjectIndustries.ProjectRaffles.Core.Services;
using ProjectIndustries.ProjectRaffles.Core.ViewModels;

namespace ProjectIndustries.ProjectRaffles.Core.EventHandlers
{
  public class CreateNotificationOnRaffleTaskCompleted
    : ApplicationEventHandlerBase<RaffleTaskCompleted>
  {
    private readonly INotificationsService _notificationsService;

    public CreateNotificationOnRaffleTaskCompleted(INotificationsService notificationsService)
    {
      _notificationsService = notificationsService;
    }

    protected override Task HandleAsync(RaffleTaskCompleted @event, CancellationToken ct)
    {
      var task = @event.Task;
      var isSuccessful = task.Status.Kind == RaffleStatusKind.Succeeded;
      var notification = new NotificationViewModel
      {
        IsSuccessful = isSuccessful,
        Title = isSuccessful ? "Entered raffle" : "Failed raffle entry",
        Description = task.ProductName
      };

      _notificationsService.Add(notification);
      return Task.CompletedTask;
    }
  }
}