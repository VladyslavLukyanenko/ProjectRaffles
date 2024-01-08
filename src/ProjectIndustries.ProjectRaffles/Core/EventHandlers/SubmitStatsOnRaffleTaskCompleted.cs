using System.Threading;
using System.Threading.Tasks;
using ProjectIndustries.ProjectRaffles.Core.Events;
using ProjectIndustries.ProjectRaffles.Core.Services;

namespace ProjectIndustries.ProjectRaffles.Core.EventHandlers
{
  public class SubmitStatsOnRaffleTaskCompleted : ApplicationEventHandlerBase<RaffleTaskCompleted>
  {
    private readonly IStatsService _statsService;
    private readonly ISubmissionStatsEntryFactory _submissionStatsEntryFactory;

    public SubmitStatsOnRaffleTaskCompleted(IStatsService statsService,
      ISubmissionStatsEntryFactory submissionStatsEntryFactory)
    {
      _statsService = statsService;
      _submissionStatsEntryFactory = submissionStatsEntryFactory;
    }

    protected override async Task HandleAsync(RaffleTaskCompleted @event, CancellationToken ct)
    {
      var entry = _submissionStatsEntryFactory.CreateEntry(@event.Task);
      await _statsService.SubmitStatsAsync(entry, ct);
    }
  }
}