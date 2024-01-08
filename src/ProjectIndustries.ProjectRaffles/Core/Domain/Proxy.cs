using System;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using LiteDB;
using Newtonsoft.Json;

namespace ProjectIndustries.ProjectRaffles.Core.Domain
{
  public class Proxy : Entity
  {
    public static readonly Regex IpAddressRegex =
      new Regex(@"^((25[0-5]|(2[0-4]|1[0-9]|[1-9]|)[0-9])(\.(?!$)|$)){4}$", RegexOptions.Compiled);

    public static readonly Regex UrlRegex =
      new Regex(
        @"[-a-zA-Z0-9@:%._\+~#=]{1,256}\.[a-zA-Z0-9()]{1,6}\b([-a-zA-Z0-9()@:%_\+.~#?&//=]*)",
        RegexOptions.Compiled);

    public static readonly Regex LocalhostRegex =
      new Regex(@"\blocalhost\b([-a-zA-Z0-9()@:%_\+.~#?&//=]*)", RegexOptions.Compiled);

    public Proxy(string username, string password, string url)
      : this(Guid.NewGuid(), username, password, url)
    {
    }
    
    [BsonCtor, JsonConstructor]
    public Proxy(Guid id, string username, string password, string url)
      : base(id)
    {
      Password = password;
      Username = username;
      Url = url;
    }

    public string Password { get; set; }
    public string Username { get; set; }
    public string Url { get; set; }
    
    [JsonIgnore]
    public bool IsAvailable { get; set; } = true;


    public static bool TryParse(string raw, out Proxy proxy)
    {
      proxy = null;
      if (string.IsNullOrEmpty(raw) ||
          !(UrlRegex.IsMatch(raw) || LocalhostRegex.IsMatch(raw) || IpAddressRegex.IsMatch(raw)))
      {
        return false;
      }

      raw = ExtractProtocol(raw, out var protocol);
      var tokens = raw.Split(':', StringSplitOptions.RemoveEmptyEntries).Select(t => t.Trim('/')).ToArray();
      if (!IsValidRawInput(tokens, out var containsPort))
      {
        return false;
      }

      var rawUrl = containsPort ? $"{tokens[0]}:{tokens[1]}" : tokens[0];
      if (!string.IsNullOrEmpty(protocol))
      {
        rawUrl = protocol + "://" + rawUrl;
      }
      //
      // if (!Uri.TryCreate(rawUrl, UriKind.RelativeOrAbsolute, out var url))
      // {
      //   return false;
      // }

      string uname = null, pwd = null;
      if (ContainsUnamePwd(containsPort, tokens))
      {
        uname = containsPort ? tokens[2] : tokens[1];
        if (ContainsPwd(containsPort, tokens))
        {
          pwd = containsPort ? tokens[3] : tokens[2];
        }
      }


      proxy = new Proxy(uname, pwd, rawUrl);
      return true;
    }

    private static bool ContainsPwd(bool containsPort, string[] tokens)
    {
      return containsPort && tokens.Length == 4 || !containsPort && tokens.Length == 3;
    }

    private static bool ContainsUnamePwd(bool containsPort, string[] tokens)
    {
      return containsPort && tokens.Length >= 3 || !containsPort && tokens.Length >= 2;
    }

    private static string ExtractProtocol(string raw, out string protocol)
    {
      var idx = raw?.IndexOf("//");
      if (idx > -1)
      {
        protocol = raw.Substring(0, idx.Value - 1);
        return raw.Substring(idx.Value + 2);
      }

      protocol = null;
      return raw;
    }

    private static bool IsValidRawInput(string[] tokens, out bool containsPort)
    {
      var pwdToken = tokens.Skip(1).FirstOrDefault();
      containsPort = !string.IsNullOrEmpty(pwdToken) && int.TryParse(tokens[1].Trim('/'), out _);

      // contains port - [1_url:2_port:3_uname:4_pwd] or [1_url:2_uname:3_pwd] 
      return !(tokens.Length > 4 || !containsPort && tokens.Length > 3);
    }

    public IWebProxy ToWebProxy()
    {
      var address = new Uri(Url.Contains("://") ? Url : "http://" + Url, UriKind.Absolute);
      if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password))
      {
        return new WebProxy(address);
      }

      return new WebProxy(address, false, Array.Empty<string>(), new NetworkCredential(Username, Password));
    }
  }
}