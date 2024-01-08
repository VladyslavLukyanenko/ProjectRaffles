using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using ProjectIndustries.ProjectRaffles.Core.Domain.Fields;

namespace ProjectIndustries.ProjectRaffles.Core.Modules
{
  public abstract class FieldsPresetBase : IEnumerable<Field>
  {
    private static readonly IDictionary<Type, FieldInfo[]> DeclaredFieldsCache =
      new Dictionary<Type, FieldInfo[]>();

    private static readonly IDictionary<Type, SemaphoreSlim> Gates = new Dictionary<Type, SemaphoreSlim>();

    protected FieldsPresetBase()
    {
      Gates[GetType()] = new SemaphoreSlim(1, 1);
    }

    public IEnumerator<Field> GetEnumerator()
    {
      var type = GetType().GetTypeInfo();
      var gates = Gates[type];
      FieldInfo[] fieldInfos;
      try
      {
        gates.Wait();
        if (!DeclaredFieldsCache.TryGetValue(type, out fieldInfos))
        {
          fieldInfos = type.DeclaredFields.ToArray();
          DeclaredFieldsCache[type] = fieldInfos;
        }
      }
      finally
      {
        gates.Release();
      }

      foreach (var fieldInfo in fieldInfos)
      {
        var field = (Field) fieldInfo.GetValue(this);
        if (field == null)
        {
          continue;
        }

        yield return field;
      }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }
  }
}