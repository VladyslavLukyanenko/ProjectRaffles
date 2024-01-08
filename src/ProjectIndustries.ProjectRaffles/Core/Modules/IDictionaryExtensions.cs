using System;
using System.Collections.Generic;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;

namespace ProjectIndustries.ProjectRaffles.Core.Modules
{
  // ReSharper disable once InconsistentNaming
  public static class IDictionaryExtensions
  {
    public static TField GetValueOrField<TField>(this IDictionary<string, object> args, Func<TField> fieldFactory)
      where TField : Field
    {
      var field = fieldFactory();
      if (args.TryGetValue(field.SystemName, out var value))
      {
        field.Value = value;
      }

      return field;
    }

    public static void Export(this IDictionary<string, object> args, IEnumerable<Field> fields)
    {
      foreach (var field in fields)
      {
        if (args.TryGetValue(field.SystemName, out var value))
        {
          field.Value = value;
        }
      }
    }
    public static IEnumerable<Field> Export(this IDictionary<string, object> args, params Field[] fields)
    {
      foreach (var field in fields)
      {
        if (args.TryGetValue(field.SystemName, out var value))
        {
          field.Value = value;
        }
        else
        {
          yield return field;
        }
      }
    }
  }
}