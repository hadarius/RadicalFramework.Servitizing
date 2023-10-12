using Microsoft.EntityFrameworkCore.Query;
using Microsoft.OData.Client;
using Radical.Uniques;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Radical.Servitizing.Repository.Client.Linked;

public class LinkedRepositoryExpressionProvider<TEntity> : IAsyncQueryProvider
    where TEntity : class, IUniqueIdentifiable
{
    private readonly Type queryType;
    private IQueryable<TEntity> query;

    public LinkedRepositoryExpressionProvider(DataServiceQuery<TEntity> targetDsSet)
    {
        queryType = typeof(LinkedRepository<>);
        query = targetDsSet;
    }

    public IQueryable CreateQuery(Expression expression)
    {
        var elementType = expression.Type.GetEnumerableElementType();
        try
        {
            return queryType.MakeGenericType(elementType).New<IQueryable>(this, expression);
        }
        catch (TargetInvocationException tie)
        {
            throw tie.InnerException;
        }
    }

    public IQueryable<T> CreateQuery<T>(Expression expression)
    {
        return queryType
            .MakeGenericType(expression.Type.GetEnumerableElementType())
            .New<IQueryable<T>>(this, expression);
    }

    public object Execute(Expression expression)
    {
        try
        {
            return GetType()
                .GetGenericMethod(nameof(Execute))
                .Invoke(this, new[] { expression });
        }
        catch (TargetInvocationException tie)
        {
            throw tie.InnerException;
        }
    }

    public TResult Execute<TResult>(Expression expression)
    {
        IQueryable<TEntity> newRoot = query;
        var treeCopier = new LinkedRepositoryExpressionVisitor(newRoot);
        var newExpressionTree = treeCopier.Visit(expression);
        var isEnumerable =
            typeof(TResult).IsGenericType
            && typeof(TResult).GetGenericTypeDefinition() == typeof(IEnumerable<>);
        if (isEnumerable)
        {
            return (TResult)newRoot.Provider.CreateQuery(newExpressionTree);
        }
        var result = newRoot.Provider.Execute(newExpressionTree);
        return (TResult)result;
    }

    public TResult ExecuteAsync<TResult>(
        Expression expression,
        CancellationToken cancellationToken = default
    )
    {
        return Task.FromResult(Execute<TResult>(expression)).Result;
    }
}
