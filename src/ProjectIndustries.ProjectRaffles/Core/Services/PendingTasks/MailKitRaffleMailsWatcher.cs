using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;
using System.Threading;
using MailKit;
using MailKit.Net.Imap;
using ProjectIndustries.ProjectRaffles.Core.Domain;

namespace ProjectIndustries.ProjectRaffles.Core.Services.PendingTasks
{
  public class MailKitRaffleMailsWatcher : IRaffleMailsWatcher
  {
    private readonly IPendingRaffleTaskService _taskService;
    private readonly IMailsWatcher _mailsWatcher;

    public MailKitRaffleMailsWatcher(IPendingRaffleTaskService taskService, IMailsWatcher mailsWatcher)
    {
      _taskService = taskService;
      _mailsWatcher = mailsWatcher;
    }

    public async IAsyncEnumerable<IncomeMailMessage> WatchAsync(PendingRaffleTask pendingTask,
      [EnumeratorCancellation] CancellationToken ct)
    {
      using (var client = new ImapClient(new ProtocolLogger(Console.OpenStandardOutput(), true)))
      {
        var creds = pendingTask.Email.GetImapCredentials();
        var cfg = creds.ImapConfig;

        await client.ConnectAsync(cfg.Host, cfg.Port, cfg.UseSsl, ct);
        await client.AuthenticateAsync(creds.Login, creds.Password, ct);

        var inbox = client.Inbox;
        await inbox.OpenAsync(FolderAccess.ReadWrite, ct);

        if (!pendingTask.LastCheckedEmailId.HasValue)
        {
          // this means we just started checking. Raffle just submitted
          pendingTask.LastCheckedEmailId = inbox.UidNext?.Id ?? 0;
          await _taskService.SaveAsync(pendingTask, ct);
        }
        
        var updateStream = new Subject<Unit>();

        IDisposable subscr = updateStream.AsObservable()
          .Throttle(TimeSpan.FromMilliseconds(500))
          .Subscribe(async _ => await _taskService.SaveAsync(pendingTask, ct));

        await foreach (IncomeMailMessage message in _mailsWatcher.WatchAsync(pendingTask.Email,
          pendingTask.LastCheckedEmailId, ct))
        {
          pendingTask.LastCheckedEmailId = message.Id;
          yield return message;
          updateStream.OnNext(Unit.Default);
        }
        
        subscr.Dispose();
        await _taskService.SaveAsync(pendingTask, ct);
      }
    }
  }
}