using System;
using System.Linq;
using NodaTime;

namespace ProjectIndustries.ProjectRaffles.Core.Infra
{
  public static class TypeAware
  {
    private static readonly Type[] PrimitiveTypes;

    static TypeAware()
    {
      var types = new[]
      {
        typeof(string), typeof(Guid), typeof(decimal), typeof(DateTime), typeof(DateTimeOffset),
        typeof(TimeSpan), typeof(Instant), typeof(LocalDate), typeof(LocalDateTime), typeof(OffsetDate),
        typeof(OffsetDateTime), typeof(OffsetTime), typeof(LocalTime), typeof(Duration), typeof(Period),
        typeof(ZonedDateTime), typeof(Offset)
      };


      var nullTypes = from t in types
        where t.IsValueType
        select typeof(Nullable<>).MakeGenericType(t);

      PrimitiveTypes = types.Concat(nullTypes).ToArray();
    }

    public static bool IsPrimitive(Type target)
    {
      return CheckIfPrimitive(target) || IsEnum(target) || PrimitiveTypes.Any(x => x.IsAssignableFrom(target));
    }

    private static bool CheckIfPrimitive(Type target)
    {
      return target.IsPrimitive || target.IsValueType && target.GenericTypeArguments.Any(_ => _.IsPrimitive);
    }

    private static bool IsEnum(Type type)
    {
      return type.IsEnum || type.IsValueType && type.GenericTypeArguments.Any(_ => _.IsEnum);
    }
  }
}