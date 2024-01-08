using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using FastExpressionCompiler;
using ProjectIndustries.ProjectRaffles.Core.Modules;

namespace ProjectIndustries.ProjectRaffles.Core.Domain.Fields.DynamicValuesPicker
{
  public static class DynamicValuesGroupFactory
  {
    private static readonly Regex ToSentenceRegex =
      new Regex(@"((?<=\p{Ll})\p{Lu})|((?!\A)\p{Lu}(?>\p{Ll}))", RegexOptions.Compiled);

    public static DynamicValuesGroup CreateRange<T>(string name,
      Func<IRaffleExecutionContext, T> instanceProvider,
      params Expression<Func<T, object>>[] propertySelectors)
    {
      var resolvers = new List<IDynamicValueResolver>(propertySelectors.Length);
      foreach (var expression in propertySelectors)
      {
        var resolver = Create(instanceProvider, expression);

        resolvers.Add(resolver);
      }

      return new DynamicValuesGroup(name, resolvers);
    }

    public static IDynamicValueResolver Create<T>(Func<IRaffleExecutionContext, T> instanceProvider,
      Expression<Func<T, object>> expression, string name = null) => expression.Body switch
    {
      MemberExpression memExpr => CreateFromMemberExpression(instanceProvider, expression, memExpr, name),
      UnaryExpression ue => CreateFromMemberExpression(instanceProvider, expression, (MemberExpression) ue.Operand,
        name),
      _ => throw new NotSupportedException(
        "Supported only MemberExpression and UnaryExpression for dynamic values resolver. Provided: "
        + expression.Body.GetType().Name)
    };

    private static IDynamicValueResolver CreateFromMemberExpression<T>(
      Func<IRaffleExecutionContext, T> instanceProvider, Expression<Func<T, object>> expression,
      MemberExpression memExpr,
      string name = null)
    {
      var valueResolver = expression.CompileFast();
      var memberResolver = new SimpleDynamicValueResolver(name ?? ToSentenceRegex.Replace(memExpr.Member.Name, " $0"),
        context => valueResolver(instanceProvider(context))?.ToString());
      return memberResolver;
    }
  }
}