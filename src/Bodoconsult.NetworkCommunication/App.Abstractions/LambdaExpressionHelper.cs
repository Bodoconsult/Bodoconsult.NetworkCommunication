// Copyright (c) Bodoconsult EDV-Dienstleistungen. All rights reserved.

using System.Linq.Expressions;

namespace Bodoconsult.NetworkCommunication.App.Abstractions;

/// <summary>
/// Helper class for lamdba expressions
/// </summary>
public static class LambdaExpressionHelper
{
    /// <summary>
    /// Makes expression for a specific property
    /// </summary>
    /// <typeparam name="TSource">Source object</typeparam>
    /// <param name="propertyName">Proeprty name</param>
    /// <returns></returns>

    public static Expression<Func<TSource, object>> GetExpression<TSource>(string propertyName)
    {
        var param = Expression.Parameter(typeof(TSource), "x");
        Expression conversion = Expression.Convert(Expression.Property
            (param, propertyName), typeof(object));   //important to use the Expression.Convert
        return Expression.Lambda<Func<TSource, object>>(conversion, param);
    }

    /// <summary>
    /// Makes delegate for specific property
    /// </summary>
    /// <typeparam name="TSource">Source object</typeparam>
    /// <param name="propertyName">Property name</param>
    /// <returns></returns>
    public static Func<TSource, object> GetFunc<TSource>(string propertyName)
    {
        return GetExpression<TSource>(propertyName).Compile();  //only need compiled expression
    }

    /// <summary>
    /// OrderBy from Enumerable&lt;TSource&gt; source
    /// </summary>
    /// <typeparam name="TSource">Source type</typeparam>
    /// <param name="source">Source object</param>
    /// <param name="propertyName">Proeprty name</param>
    /// <returns><see cref="IOrderedQueryable"/> instance</returns>
    public static IOrderedEnumerable<TSource> OrderBy<TSource>(this IEnumerable<TSource> source, string propertyName)
    {
        return source.OrderBy(GetFunc<TSource>(propertyName));
    }

    /// <summary>
    /// OrderBy from IQueryable&lt;TSource&gt; source
    /// </summary>
    /// <typeparam name="TSource">Source type</typeparam>
    /// <param name="source">Source object</param>
    /// <param name="propertyName">Proeprty name</param>
    /// <returns><see cref="IOrderedQueryable"/> instance</returns>
    public static IOrderedQueryable<TSource> OrderBy<TSource>(this IQueryable<TSource> source, string propertyName)
    {
        return source.OrderBy(GetExpression<TSource>(propertyName));
    }
}