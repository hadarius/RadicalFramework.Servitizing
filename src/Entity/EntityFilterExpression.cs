﻿using System;
using System.Collections.Generic;
using System.Globalization;
using Radical.Instant;
using System.Linq;
using System.Linq.Expressions;
using Radical.Uniques;

namespace Radical.Servitizing.Entity;

using Instant.Proxies;
using Instant.Rubrics;
using DTO;

public class EntityFilterExpression<TEntity>
{
    private NumberFormatInfo nfi = new NumberFormatInfo();
    private ProxyCreator sleeve;
    public IRubrics Rubrics;
    private Expression<Func<TEntity, bool>> filterExpression { get; set; }

    public IList<EntityFilter<TEntity>> FilterItems { get; } = new List<EntityFilter<TEntity>>();

    public EntityFilterExpression()
    {
        nfi.NumberDecimalSeparator = ".";
        sleeve = ProxyFactory.GetCompiledCreator<TEntity>();
        Rubrics = sleeve.Rubrics;
    }
    public EntityFilterExpression(params EntityFilter<TEntity>[] filterItems) : this()
    {
        filterItems.ForEach(fi => Add(fi)).ToArray();
    }
    public EntityFilterExpression(IEnumerable<EntityFilter<TEntity>> filterItems) : this()
    {
        filterItems.ForEach(fi => Add(fi)).ToArray();
    }
    public EntityFilterExpression(IEnumerable<FilterDTO> filterItems) : this()
    {
        filterItems.ForEach(fi => Add(new EntityFilter<TEntity>(fi))).ToArray();
    }

    public Expression<Func<TEntity, bool>> Create()
    {
        return Create(FilterItems);
    }
    public Expression<Func<TEntity, bool>> Create(IEnumerable<EntityFilter<TEntity>> filterItems)
    {
        Expression<Func<TEntity, bool>> exps = null;
        //filterItems.ForEach(fi => Add(fi));
        filterExpression = null;
        LogicOperand logic = LogicOperand.And;
        foreach (EntityFilter<TEntity> ft in FilterItems)
        {
            exps = null;
            if (ft.Operand != MathOperand.Contains)
            {
                if (filterExpression != null)
                {
                    if (logic != LogicOperand.Or)
                        filterExpression = filterExpression.And(ConvertItem(ft, exps));
                    else
                        filterExpression = filterExpression.Or(ConvertItem(ft, exps));
                }
                else
                    filterExpression = ConvertItem(ft, exps);
                logic = ft.Logic;
            }
            else
            {
                HashSet<int> list = new HashSet<int>(ft.Value.GetType() == typeof(string) ? ft.Value.ToString().Split(';')
                                                     .Select(p => Convert.ChangeType(p, ft.Rubric.RubricType).GetHashCode()) :
                                                     ft.Value.GetType() == typeof(List<object>) ? ((List<object>)ft.Value)
                                                     .Select(p => Convert.ChangeType(p, ft.Rubric.RubricType).GetHashCode()) : null);

                if (list != null && list.Count > 0)
                    exps = r => list.Contains(r.ValueOf(ft.Rubric.RubricName).GetHashCode());

                if (filterExpression != null)
                    if (logic != LogicOperand.Or)
                        filterExpression = filterExpression.And(exps);
                    else
                        filterExpression = filterExpression.Or(exps);
                else
                    filterExpression = exps;
                logic = ft.Logic;
            }
        }
        return filterExpression;
    }
    public Expression<Func<TEntity, bool>> ConvertItem(EntityFilter<TEntity> filterItem, Expression<Func<TEntity, bool>> expression = null)
    {
        var ft = filterItem;
        var ex = expression;
        if (ft.Value != null)
        {
            object value = filterItem.Value;
            MathOperand compare = ft.Operand;
            Type type = ft.Rubric.RubricType;
            bool isNumeric = type == typeof(IUnique) ||
                             type == typeof(string) ||
                             type == typeof(DateTime) ||
                             type == typeof(Enum) ?
                             false : true;


            if (compare != MathOperand.Like && compare != MathOperand.NotLike)
            {
                switch (compare)
                {
                    case MathOperand.Equal:

                        ex = r => r.ValueOf(ft.Rubric.Name).Equals(value);
                        break;

                    case MathOperand.GreaterOrEqual:

                        ex = r => r.ValueOf(ft.Rubric.Name) != null ? !isNumeric ?
                        r.ValueOf(ft.Rubric.Name).ComparableUInt64(ft.Rubric.RubricType)
                                                  >=
                        value.ComparableUInt64(ft.Rubric.RubricType) :
                        r.ValueOf(ft.Rubric.Name).ComparableDouble(ft.Rubric.RubricType)
                                                  >=
                        value.ComparableDouble(ft.Rubric.RubricType) : false;
                        break;

                    case MathOperand.Greater:

                        ex = r => r.ValueOf(ft.Rubric.Name) != null ? !isNumeric ?
                        r.ValueOf(ft.Rubric.Name).ComparableUInt64(ft.Rubric.RubricType)
                                                 >
                        value.ComparableUInt64(ft.Rubric.RubricType) :
                        r.ValueOf(ft.Rubric.Name).ComparableDouble(ft.Rubric.RubricType)
                                                 >
                        value.ComparableDouble(ft.Rubric.RubricType) : false;
                        break;

                    case MathOperand.LessOrEqual:

                        ex = r => r.ValueOf(ft.Rubric.Name) != null ? !isNumeric ?
                        r.ValueOf(ft.Rubric.Name).ComparableUInt64(ft.Rubric.RubricType)
                                                <=
                        value.ComparableUInt64(ft.Rubric.RubricType) :
                        r.ValueOf(ft.Rubric.Name).ComparableDouble(ft.Rubric.RubricType)
                                                <=
                        value.ComparableDouble(ft.Rubric.RubricType) : false;
                        break;

                    case MathOperand.Less:

                        ex = r => r.ValueOf(ft.Rubric.Name) != null ? !isNumeric ?
                        r.ValueOf(ft.Rubric.Name).ComparableUInt64(ft.Rubric.RubricType)
                                                <
                        value.ComparableUInt64(ft.Rubric.RubricType) :
                        r.ValueOf(ft.Rubric.Name).ComparableDouble(ft.Rubric.RubricType)
                                                <
                        value.ComparableDouble(ft.Rubric.RubricType) : false;
                        break;
                    default:
                        break;
                }
            }
            else if (compare != MathOperand.NotLike)

                ex = r => r.ValueOf(ft.Rubric.Name) != null
                ? Convert.ChangeType(r.ValueOf(ft.Rubric.Name), ft.Rubric.RubricType).ToString()
                    .Contains(Convert.ChangeType(value, ft.Rubric.RubricType).ToString())
                      : false;
            else
                ex = r => r.ValueOf(ft.Rubric.Name) != null
                ? !Convert.ChangeType(r.ValueOf(ft.Rubric.Name), ft.Rubric.RubricType).ToString()
                    .Contains(Convert.ChangeType(value, ft.Rubric.RubricType).ToString())
                      : false;
        }
        return ex;
    }

    public EntityFilter<TEntity> Add(EntityFilter<TEntity> item)
    {
        item.Assign(this);
        FilterItems.Add(item);
        return item;
    }
    public IEnumerable<EntityFilter<TEntity>> Add(IEnumerable<EntityFilter<TEntity>> filterItems)
    {
        filterItems.ForEach(fi => Add(fi));
        return FilterItems;
    }
}