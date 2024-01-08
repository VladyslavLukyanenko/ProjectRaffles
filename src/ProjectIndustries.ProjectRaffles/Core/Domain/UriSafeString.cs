using System;
using System.Linq;
using System.Text;
using Unidecode.NET;

namespace ProjectIndustries.ProjectRaffles.Core.Domain
{
  public class UriSafeString
  {
    protected UriSafeString(string value)
    {
      if (string.IsNullOrWhiteSpace(value))
      {
        throw new ArgumentException(nameof(value));
      }

      Value = value;
    }

    public string Value { get; private set; }

    /// <summary>
    /// Produces optional, URL-friendly version of a title, "like-this-one". 
    /// hand-tuned for speed, reflects performance refactoring contributed
    /// by John Gietzen (user otac0n) 
    /// </summary>
    public static UriSafeString Create(params string[] tokens)
    {
      return Create(0, 80, tokens);
    }

    /// <summary>
    /// Produces optional, URL-friendly version of a title, "like-this-one". 
    /// hand-tuned for speed, reflects performance refactoring contributed
    /// by John Gietzen (user otac0n) 
    /// </summary>
    public static UriSafeString Create(int minLength = 0, int maxLength = 80, params string[] tokens)
    {
      var slug = Slugify(minLength, maxLength, tokens);

      return new UriSafeString(slug);
    }

    public static string Slugify(int minLength, int maxLength, params string[] tokens)
    {
      string title = string.Join("-", tokens.Where(t => !string.IsNullOrEmpty(t)));
      if (title.Length < minLength)
      {
        throw new ArgumentException("MinLength");
      }

      if (string.IsNullOrWhiteSpace(title))
      {
        throw new ArgumentNullException(nameof(tokens));
      }

      int len = title.Length;
      bool prevdash = false;
      var sb = new StringBuilder(len);
      char c;

      for (int i = 0; i < len; i++)
      {
        c = title[i];
        if ((c >= 'a' && c <= 'z') || (c >= '0' && c <= '9'))
        {
          sb.Append(c);
          prevdash = false;
        }
        else if (c >= 'A' && c <= 'Z')
        {
          // tricky way to convert to lowercase
          sb.Append((char) (c | 32));
          prevdash = false;
        }
        else if (c == ' ' || c == ',' || c == '.' || c == '/' ||
                 c == '\\' || c == '-' || c == '_' || c == '=')
        {
          if (!prevdash && sb.Length > 0)
          {
            sb.Append('-');
            prevdash = true;
          }
        }
        else if (c >= 128)
        {
          int prevlen = sb.Length;
          sb.Append(RemapInternationalCharToAscii(c));
          if (prevlen != sb.Length)
          {
            prevdash = false;
          }
        }

        if (i == maxLength)
        {
          break;
        }
      }

      string slugValue;

      if (prevdash)
      {
        slugValue = sb.ToString().Substring(0, sb.Length - 1);
      }
      else
      {
        slugValue = sb.ToString();
      }

      if (string.IsNullOrEmpty(slugValue) || slugValue.Length < minLength)
      {
        throw new InvalidOperationException("Empty slug created from provided params");
      }

      return slugValue;
    }

    public static string RemapInternationalCharToAscii(char c)
    {
      string s = c.ToString().ToLowerInvariant();
      if ("àåáâäãåą".Contains(s))
      {
        return "a";
      }
      else if ("èéêëę".Contains(s))
      {
        return "e";
      }
      else if ("ìíîïı".Contains(s))
      {
        return "i";
      }
      else if ("òóôõöøőð".Contains(s))
      {
        return "o";
      }
      else if ("ùúûüŭů".Contains(s))
      {
        return "u";
      }
      else if ("çćčĉ".Contains(s))
      {
        return "c";
      }
      else if ("żźž".Contains(s))
      {
        return "z";
      }
      else if ("śşšŝ".Contains(s))
      {
        return "s";
      }
      else if ("ñń".Contains(s))
      {
        return "n";
      }
      else if ("ýÿ".Contains(s))
      {
        return "y";
      }
      else if ("ğĝ".Contains(s))
      {
        return "g";
      }
      else if (c == 'ř')
      {
        return "r";
      }
      else if (c == 'ł')
      {
        return "l";
      }
      else if (c == 'đ')
      {
        return "d";
      }
      else if (c == 'ß')
      {
        return "ss";
      }
      else if (c == 'Þ')
      {
        return "th";
      }
      else if (c == 'ĥ')
      {
        return "h";
      }
      else if (c == 'ĵ')
      {
        return "j";
      }
      else
      {
        return "";
      }
    }

    public static UriSafeString FromSource(string source)
    {
      return new UriSafeString(source?.ToLowerInvariant());
    }
  }
}