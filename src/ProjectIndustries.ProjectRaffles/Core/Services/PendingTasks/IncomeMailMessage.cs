using System.Collections.Generic;
using System.Linq;
using NodaTime;

namespace ProjectIndustries.ProjectRaffles.Core.Services.PendingTasks
{
  public class IncomeMailMessage
  {
    public IncomeMailMessage(uint id, IEnumerable<string> receiverEmails, IEnumerable<string> senderEmails, string subject,
      string content, Instant receivedAt, IEnumerable<KeyValuePair<string, string>> headers)
    {
      Id = id;
      ReceiverEmails = receiverEmails.ToList().AsReadOnly();
      SenderEmails = senderEmails.ToList().AsReadOnly();
      Subject = subject;
      Content = content;
      ReceivedAt = receivedAt;
      Headers = headers.ToList().AsReadOnly();
    }

    public uint Id { get; private set; }
    public IReadOnlyList<KeyValuePair<string, string>> Headers { get; private set; }
    public IReadOnlyList<string> ReceiverEmails { get; }
    public IReadOnlyList<string> SenderEmails { get; }
    public string Subject { get; }
    public string Content { get; }
    public Instant ReceivedAt { get; }
  }
}