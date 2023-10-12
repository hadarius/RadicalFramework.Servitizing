using System;
using Radical.Instant;
using System.Linq.Expressions;

namespace Radical.Servitizing.Entity;

using Instant.Rubrics;
using Instant.Proxies;
using DTO;

public class EntitySort<TEntity>
{
    private EntitySortExpression<TEntity> sortExpression;

    public EntitySort()
    {
    }
    public EntitySort(Expression<Func<TEntity, object>> expressionItem, SortDirection direction = SortDirection.Asc)
    {
        ExpressionItem = expressionItem;
        Direction = direction;
    }
    public EntitySort(MemberRubric sortedRubric, SortDirection direction = SortDirection.Asc)
    {
        Direction = direction;
        Rubric = sortedRubric;
        Property = Rubric.Name;
    }
    public EntitySort(string rubricName, string direction = "Asc")
    {
        Property = rubricName;
        SortDirection sortDirection;
        Enum.TryParse(direction, true, out sortDirection);
        Direction = sortDirection;
    }
    public EntitySort(SortDTO item) : this(item.Property, item.Direction)
    {
    }

    public Expression<Func<TEntity, object>> ExpressionItem { get; set; }

    public SortDirection Direction { get; set; }

    public int Position { get; set; }

    public string Property { get; set; }

    public MemberRubric Rubric { get; set; }

    public void Assign(EntitySortExpression<TEntity> sortExpression)
    {
        var fe = sortExpression;
        this.sortExpression = fe;
        if (fe.Rubrics.TryGet(Property, out MemberRubric rubric))
        {
            Rubric = rubric;
            ExpressionItem = e => e.ValueOf(Property);
        }
    }

    public bool Compare(EntitySort<TEntity> term)
    {
        if (Property != term.Property || Direction != term.Direction)
            return false;

        return true;
    }

}
