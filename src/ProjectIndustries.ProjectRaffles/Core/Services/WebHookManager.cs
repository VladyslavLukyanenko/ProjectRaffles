using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ProjectIndustries.ProjectRaffles.Core.Domain;
using ProjectIndustries.ProjectRaffles.Core.Domain.Discord;

namespace ProjectIndustries.ProjectRaffles.Core.Services
{
  public class WebHookManager : IWebHookManager
  {
    private DiscordSettings _discordSettings;
    private readonly ISoftwareInfoProvider _softwareInfoProvider;

    private static readonly HttpClient Client = new HttpClient();
    private static readonly Queue<ScheduledWebHook> Queue = new Queue<ScheduledWebHook>();

    public WebHookManager(IDiscordSettingsService discordSettingsService, ISoftwareInfoProvider softwareInfoProvider)
    {
      _softwareInfoProvider = softwareInfoProvider;
      discordSettingsService.Settings.Subscribe(s => _discordSettings = s);
    }

    public void EnqueueWebhook(PendingRaffleTask task)
    {
      var webhookObj = new DiscordWebhookBody
      {
        Embeds = new List<Embed>
        {
          Embed.CreateFrom(task, _softwareInfoProvider.CurrentSoftwareVersion)
        }
      };

      TryEnqueue(webhookObj);
    }

    public async Task<bool> TestWebhook()
    {
      if (string.IsNullOrEmpty(_discordSettings.WebHook))
      {
        return false;
      }

      var webhookObj = new DiscordWebhookBody
      {
        Content = "",
        Username = "Project Raffles",
        AvatarUrl = "https://projectindustries.gg/images/logo-project-raffles.png",
        Embeds = new List<Embed>()
      };

      webhookObj.Embeds.Add(new Embed
      {
        Author = new Author
        {
          Name = "",
          Url = "",
          IconUrl = "https://projectindustries.gg/images/logo-project-raffles.png"
        },
        Color = 0xE71A48,
        Title = "**Test Webhook** :partying_face:",
        Url = "",
        Image = new Image
        {
          Url = "https://projectindustries.gg/images/logo-project-raffles.png"
        },
        Thumbnail = new Image {Url = ""},
        Footer = new Footer
        {
          Text = $"Project Raffles v{_softwareInfoProvider.CurrentSoftwareVersion}",
        },
        Fields = new List<Field>()
      });

      try
      {
        var webhookContent = new StringContent(JsonConvert.SerializeObject(webhookObj), Encoding.UTF8,
          "application/json");
        var webhookResp = await Client.PostAsync(_discordSettings.WebHook, webhookContent);

        return (int) webhookResp.StatusCode == 204;
      }
      catch
      {
        return false;
      }
    }


    public void TryEnqueue(DiscordWebhookBody webhookBody)
    {
      if (string.IsNullOrEmpty(_discordSettings.WebHook))
      {
        return;
      }

      Queue.Enqueue(new ScheduledWebHook(_discordSettings.WebHook, webhookBody));
    }

    public void Spawn()
    {
      Task.Factory.StartNew(async () =>
      {
        while (true)
        {
          if (Queue.Count > 0)
          {
            var item = Queue.Dequeue();
            try
            {
              var webhookContent = new StringContent(JsonConvert.SerializeObject(item.Content), Encoding.UTF8,
                "application/json");
              var webhookResp = await Client.PostAsync(item.WebHookApiUrl, webhookContent);

              if (webhookResp.StatusCode == HttpStatusCode.NotFound)
              {
                continue;
              }

              if ((int) webhookResp.StatusCode != 204)
              {
                Queue.Enqueue(item);
              }
            }
            catch
            {
              if (item != null)
              {
                Queue.Enqueue(item);
              }
            }
          }

          await Task.Delay(500);
        }

        // ReSharper disable once FunctionNeverReturns
      }, TaskCreationOptions.LongRunning);
    }

    private class ScheduledWebHook
    {
      public ScheduledWebHook(string webHookApiUrl, DiscordWebhookBody content)
      {
        WebHookApiUrl = webHookApiUrl;
        Content = content;
      }

      public string WebHookApiUrl { get; }
      public DiscordWebhookBody Content { get; }
    }
  }
}