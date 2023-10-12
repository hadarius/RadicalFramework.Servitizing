using System;
using System.Collections.Generic;
using Radical.Instant;
using Radical.Instant.Series;
using System.Linq;
using System.Linq.Expressions;
using Radical.Servitizing.DTO;

namespace Radical.Servitizing.Entity;

using Instant.Proxies;
using Instant.Rubrics;

public class EntitySortExpression<TEntity>
{
    private ProxyCreator sleeve;
    public IRubrics Rubrics;
    private Expression<Func<TEntity, object>> sortExpression { get; set; }

    public IList<EntitySort<TEntity>> SortItems { get; } = new List<EntitySort<TEntity>>();

    public EntitySortExpression()
    {
        sleeve = ProxyFactory.GetCompiledCreator<TEntity>();
        Rubrics = sleeve.Rubrics;
    }
    public EntitySortExpression(Expression<Func<TEntity, object>> expressionItem, SortDirection direction) : this()
    {
        Add(new EntitySort<TEntity>(expressionItem, direction));
    }
    public EntitySortExpression(params EntitySort<TEntity>[] sortItems) : this()
    {
        sortItems.ForEach(fi => Add(fi));
    }
    public EntitySortExpression(IEnumerable<EntitySort<TEntity>> sortItems) : this()
    {
        sortItems.ForEach(fi => Add(fi));
    }
    public EntitySortExpression(IEnumerable<SortDTO> sortItems) : this()
    {
        sortItems.ForEach(fi => Add(new EntitySort<TEntity>(fi))).ToList();
    }

    public IQueryable<TEntity> Sort(IQueryable<TEntity> query)
    {
        return Sort(query, SortItems);
    }
    public IQueryable<TEntity> Sort(IQueryable<TEntity> query, IEnumerable<EntitySort<TEntity>> sortItems)
    {

        if (sortItems != null && sortItems.Any())
        {
            if (!SortItems.Any())
                sortItems.ForEach(fi => Add(fi));

            bool first = true;
            IOrderedEnumerable<TEntity> orderedQuery = null;
            foreach (var sortItem in SortItems)
            {
                if (sortItem.Direction.Equals(SortDirection.Asc))
                {
                    orderedQuery = first ? query.OrderBy(sortItem.ExpressionItem.Compile()) : orderedQuery.ThenBy(sortItem.ExpressionItem.Compile());
                }
                else
                {
                    orderedQuery = first
                        ? query.OrderByDescending(sortItem.ExpressionItem.Compile())
                        : orderedQuery.ThenByDescending(sortItem.ExpressionItem.Compile());
                }

                first = false;
            }

            return orderedQuery.AsQueryable();
        }
        else
        {
            return query;
        }
    }

    public EntitySort<TEntity> Add(EntitySort<TEntity> item)
    {
        item.Assign(this);
        SortItems.Add(item);
        return item;
    }
    public IEnumerable<EntitySort<TEntity>> Add(IEnumerable<EntitySort<TEntity>> sortItems)
    {
        sortItems.ForEach(fi => Add(fi));
        return SortItems;
    }
}

public enum SortDirection
{
    Asc,
    Desc
}