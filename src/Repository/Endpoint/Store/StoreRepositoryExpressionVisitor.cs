﻿using System.Linq;
using System.Linq.Expressions;

namespace Radical.Servitizing.Repository.Endpoint.Store;

internal class StoreRepositoryExpressionVisitor : ExpressionVisitor
{
    private readonly IQueryable newRoot;

    public StoreRepositoryExpressionVisitor(IQueryable newRoot)
    {
        this.newRoot = newRoot;
    }

    protected override Expression VisitConstant(ConstantExpression node) =>
         node.Type.BaseType != null &&
         node.Type.BaseType.IsGenericType &&
         node.Type.BaseType.GetGenericTypeDefinition() == typeof(StoreRepository<>) ?
         Expression.Constant(newRoot) : node;

}
