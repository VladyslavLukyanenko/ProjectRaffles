using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using Microsoft.Extensions.Logging;
using MimeKit;
using NodaTime.Extensions;
using ProjectIndustries.ProjectRaffles.Core.Domain;
using ProjectIndustries.ProjectRaffles.Core.Domain.Emails;

namespace ProjectIndustries.ProjectRaffles.Core.Services.PendingTasks
{
  public class MailKitMailsWatcher : IMailsWatcher
  {
    private readonly ILogger<MailKitMailsWatcher> _logger;
    public MailKitMailsWatcher(ILogger<MailKitMailsWatcher> logger)
    {
      _logger = logger;
    }

    public async IAsyncEnumerable<IncomeMailMessage> WatchAsync(Email email, uint? startMailId = null,
      [EnumeratorCancellation] CancellationToken ct = default)
    {
      var cts = new CancellationTokenSource();
      using (var client = new ImapClient(new ProtocolLogger(Console.OpenStandardOutput(), true)))
      {
        var creds = email.GetImapCredentials();
        var cfg = creds.ImapConfig;

        await client.ConnectAsync(cfg.Host, cfg.Port, cfg.UseSsl, ct);
        await client.AuthenticateAsync(creds.Login, creds.Password, ct);

        var inbox = client.Inbox;
        await inbox.OpenAsync(FolderAccess.ReadWrite, ct);

        UniqueId? lastSeenMail = new UniqueId(startMailId ?? inbox.UidNext?.Id ?? 0);
        inbox.CountChanged += OnInboxOnCountChanged;
        while (!ct.IsCancellationRequested)
        {
          await inbox.CloseAsync(cancellationToken: ct);
          await inbox.OpenAsync(FolderAccess.ReadWrite, ct);
          if (inbox.UidNext == lastSeenMail)
          {
            await client.IdleAsync(cts.Token, ct);
          }

          var start = lastSeenMail.GetValueOrDefault(UniqueId.MaxValue);
          var range = new UniqueIdRange(start, UniqueId.MaxValue);
          var uniqueIds = await inbox.SearchAsync(range, SearchQuery.All, ct);
          uint nextMaxId = 0;
          foreach (var summary in await inbox.FetchAsync(uniqueIds,
            MessageSummaryItems.All | MessageSummaryItems.BodyStructure | MessageSummaryItems.Headers, ct))
          {
            var senderEmails = summary.Envelope.From.Mailboxes.Select(_ => _.Address);
            var receiverEmails = summary.Envelope.To.Mailboxes.Select(_ => _.Address);

            BodyPart part = summary.TextBody ?? summary.HtmlBody;
            var text = (TextPart) await inbox.GetBodyPartAsync(summary.UniqueId, part, ct);

            var headers = summary.Headers.Select(h => new KeyValuePair<string, string>(h.Field, h.Value));
            nextMaxId = Math.Max(nextMaxId, summary.UniqueId.Id);
            var message = new IncomeMailMessage(nextMaxId, receiverEmails, senderEmails, summary.NormalizedSubject,
              text.Text, summary.Date.ToInstant(), headers);

            yield return message;
          }

          lastSeenMail = new UniqueId(nextMaxId + 1);
        }

        void OnInboxOnCountChanged(object o, EventArgs eventArgs)
        {
          _logger.LogDebug("[{Email}] New message (s) received", email.Value);

          cts.Cancel();
          cts = new CancellationTokenSource();
        }
      }
    }
  }
}