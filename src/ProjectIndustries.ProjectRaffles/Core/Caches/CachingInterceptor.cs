using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using ProjectIndustries.ProjectRaffles.Core.Infra;
using ProjectIndustries.ProjectRaffles.Core.Modules;

namespace ProjectIndustries.ProjectRaffles.Core.Caches
{
  public class CachingInterceptor : IInterceptor
  {
    private static readonly MethodInfo WrapMethodDef =
      ReflectionHelper.GetGenericMethodDef<CachingInterceptor>(_ => _.Wrap<object>(default));

    private static readonly ConcurrentDictionary<string, SemaphoreSlim> Limiters =
      new ConcurrentDictionary<string, SemaphoreSlim>();

    private static readonly object Gates = new object();

    private readonly ICacheService _cacheService;

    public CachingInterceptor(ICacheService cacheService)
    {
      _cacheService = cacheService;
    }

    public void Intercept(IInvocation invocation)
    {
      if (invocation.Method.GetCustomAttribute<CacheOutputAttribute>() == null)
      {
        invocation.Proceed();
        return;
      }

      var returnType = invocation.Method.ReturnType;
      if (!typeof(Task).IsAssignableFrom(returnType) || returnType == typeof(Task))
      {
        throw new InvalidOperationException("Can't intercept void or Task methods");
      }

      var invokeType = typeof(Task).IsAssignableFrom(returnType) ? returnType.GetGenericArguments()[0] : returnType;
      CallGenericMethod(WrapMethodDef, new[] {invokeType}, new[] {invocation});
    }

    private void Wrap<T>(IInvocation invocation)
    {
      var args = string.Join("_", invocation.Arguments.Where(a => a != null && TypeAware.IsPrimitive(a.GetType())));
      string key = invocation.TargetType.FullName.Slugify()
                   + ":" + invocation.MethodInvocationTarget.Name.Slugify();

      if (!string.IsNullOrWhiteSpace(args))
      {
        key += ":" + args.Slugify();
      }

      key = "ProjectRafflesCaches:" + key;
      SemaphoreSlim limiter;
      lock (Gates)
      {
        if (!Limiters.TryGetValue(key, out limiter))
        {
          limiter = new SemaphoreSlim(1, 1);
          Limiters[key] = limiter;
        }
      }

      invocation.ReturnValue = Task.Run(async () =>
      {
        try
        {
          await limiter.WaitAsync();
          var result = await _cacheService.GetAsync<T>(key);
          if (Equals(result, default))
          {
            var pendingTask =
              (Task<T>) invocation.MethodInvocationTarget.Invoke(invocation.InvocationTarget, invocation.Arguments);
            result = await pendingTask!;
            await _cacheService.SetAsync(key, result);
          }

          return result;
        }
        finally
        {
          limiter.Release();
        }
      });
    }

    private void CallGenericMethod(MethodInfo targetMethod, Type[] typeParameters, object[] arguments)
    {
      targetMethod.MakeGenericMethod(typeParameters)
        .Invoke(this, arguments);
    }
  }
}