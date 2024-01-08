using System;
using System.Linq.Expressions;
using System.Reflection;

namespace ProjectIndustries.ProjectRaffles.Core
{
  public static class ReflectionHelper
  {
    public static MethodInfo GetGenericMethodDef<TSource>(Expression<Action<TSource>> expr)
    {
      var selectorBody = (MethodCallExpression) expr.Body;
      return selectorBody.Method.GetGenericMethodDefinition();
    }

    public static Type GetInternalType(string assemblyName, string classNamespace, string className)
    {
      Assembly assembly = Assembly.Load(assemblyName);
      var type = assembly.GetType($"{classNamespace}.{className}");
      return type;
    }


    public static object GetStaticInternalProperty(string assemblyName, string classNamespace, string className,
      string propertyName)
    {
      return GetInternalType(assemblyName, classNamespace, className)
        .GetProperty(propertyName, BindingFlags.Public | BindingFlags.Static)
        .GetValue(null, null);
    }

    public static object CreateInstanceOfInternalClass(string assemblyName, string classNamespace, string className,
      object[] ctorArgs)
    {
      Assembly assembly = Assembly.Load(assemblyName);
      return assembly.CreateInstance($"{classNamespace}.{className}", false,
        BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.CreateInstance,
        null, ctorArgs, null, null);
    }
  }
}