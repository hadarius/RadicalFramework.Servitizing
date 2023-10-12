using System.Linq;
using System.Linq.Expressions;

namespace Radical.Servitizing.Repository.Client.Linked;

internal class LinkedRepositoryExpressionVisitor : ExpressionVisitor
{
    private readonly IQueryable newRoot;

    public LinkedRepositoryExpressionVisitor(IQueryable newRoot)
    {
        this.newRoot = newRoot;
    }

    protected override Expression VisitConstant(ConstantExpression node) =>
        node.Type.BaseType != null
        && node.Type.BaseType.IsGenericType
        && node.Type.BaseType.GetGenericTypeDefinition() == typeof(LinkedRepository<>)
            ? Expression.Constant(newRoot)
            : node;
}
