using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;
using AEIG.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace AEIG.Domain.Data.Extensions;

public static class RepositoryExtensions
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public static async Task<AEIG.Domain.DynamicQueryResult<TEntity>> ToPagedAsync<TEntity>(this IQueryable<TEntity> query, AEIG.Domain.DynamicQuery filter) where TEntity : class
    {
        var pagedResult = new AEIG.Domain.DynamicQueryResult<TEntity>(filter.PageSize);

        var filterList = filter.Filter ?? ParseFilterString(filter.FilterString);
        var orderByList = filter.OrderBy ?? ParseOrderByString(filter.OrderByString);

        if (filterList != null)
            query = query.Filter(filterList, filter.Operator);

        if (filter.ResultType == DynamicQueryResultType.Paginated)
        {
            pagedResult.TotalRows = await Task.Run(() => query.Count());
            pagedResult.PageNumber = filter.PageNumber;

            if (orderByList != null)
                query = query.OrderBy(orderByList);

            query = query.Skip(filter.Skip).Take(filter.PageSize);
        }
        else if (orderByList != null)
            query = query.OrderBy(orderByList);

        if (filter.ResultType == DynamicQueryResultType.QueryString)
            pagedResult.QueryString = query.ToQueryString();
        else
            pagedResult.Result = await Task.Run(() => query.ToList());

        return pagedResult;
    }

    private static IList<AEIG.Domain.PropertyFilter>? ParseFilterString(string? filterString) =>
        string.IsNullOrWhiteSpace(filterString)
            ? null
            : JsonSerializer.Deserialize<List<AEIG.Domain.PropertyFilter>>(filterString, JsonOptions);

    private static IList<AEIG.Domain.PropertySort>? ParseOrderByString(string? orderByString) =>
        string.IsNullOrWhiteSpace(orderByString)
            ? null
            : JsonSerializer.Deserialize<List<AEIG.Domain.PropertySort>>(orderByString, JsonOptions);

    public static IQueryable<TEntity> Filter<TEntity>(this IQueryable<TEntity> query, IList<AEIG.Domain.PropertyFilter> propertiesToFilter, ExpressionType operatorType)
    {
        if (propertiesToFilter == null || propertiesToFilter.Count == 0)
            return query;

        var entityType = typeof(TEntity);
        var parameter = Expression.Parameter(typeof(TEntity), "p");
        var expressions = new List<Expression>();

        var groupedProperties = propertiesToFilter.GroupBy(p => new { p.Name, p.Condition }).ToList();
        foreach (var groupedProperty in groupedProperties)
        {
            var propertyToFilter = groupedProperty.First();
            var propertyInfo = entityType.GetProperty(propertyToFilter.Name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (propertyInfo == null)
                continue;

            var member = Expression.Property(parameter, propertyInfo.Name);
            Expression? expression = null;

            if (groupedProperty.Count() == 1)
            {
                Expression? valueComparator = null;
                if (propertyToFilter.Condition == ExpressionType.Equal && propertyInfo.PropertyType == typeof(string))
                {
                    var value = ChangeType(propertyToFilter.GetValue(), propertyInfo.PropertyType);
                    var propertyValue = new { value };
                    valueComparator = Expression.Property(Expression.Constant(propertyValue), nameof(value));
                }
                else if (propertyToFilter.Condition != ExpressionType.OrAssign)
                {
                    valueComparator = Expression.Constant(ChangeType(propertyToFilter.GetValue(), propertyInfo.PropertyType), propertyInfo.PropertyType);
                }

                switch (propertyToFilter.Condition)
                {
                    case ExpressionType.OrAssign:
                        expression = GetContainsExpression(groupedProperty.ToList(), propertyInfo, member);
                        break;
                    case ExpressionType.Equal:
                        expression = Expression.Equal(member, valueComparator!);
                        break;
                    case ExpressionType.NotEqual:
                        expression = Expression.NotEqual(member, valueComparator!);
                        break;
                    case ExpressionType.GreaterThan:
                        expression = Expression.GreaterThan(member, valueComparator!);
                        break;
                    case ExpressionType.GreaterThanOrEqual:
                        expression = Expression.GreaterThanOrEqual(member, valueComparator!);
                        break;
                    case ExpressionType.LessThan:
                        expression = Expression.LessThan(member, valueComparator!);
                        break;
                    case ExpressionType.LessThanOrEqual:
                        expression = Expression.LessThanOrEqual(member, valueComparator!);
                        break;
                }
            }
            else
                expression = GetContainsExpression(groupedProperty.ToList(), propertyInfo, member);

            if (expression is not null)
                expressions.Add(expression);
        }

        if (expressions.Count == 0)
            return query;

        var condition = Expression.Lambda<Func<TEntity, bool>>(GenerateExpression(expressions, operatorType), parameter);
        return query.Where(condition);
    }

    private static Expression GetContainsExpression(List<AEIG.Domain.PropertyFilter> groupedProperties, PropertyInfo propertyInfo, MemberExpression member)
    {
        if (propertyInfo.PropertyType == typeof(string))
        {
            groupedProperties.ForEach(x => x.Value = x.Value.ToString()!.ToLower());
            var methodTolower = typeof(string).GetMethod("ToLower", Type.EmptyTypes)!;
            var callToLower = Expression.Call(member, methodTolower);

            var containsMethod = typeof(string).GetMethods()
                .First(p =>
                    p.Name == "Contains" &&
                    p.GetParameters().Length == 1 &&
                    p.GetParameters()[0].ParameterType == typeof(string));
            var expressions = new List<Expression>();

            groupedProperties.ForEach(p =>
                expressions.Add(Expression.Call(callToLower, containsMethod, Expression.Constant(ChangeType(p.GetValue(), propertyInfo.PropertyType)))));

            return GenerateExpression(expressions, ExpressionType.Or);
        }

        var listType = typeof(List<>).MakeGenericType(propertyInfo.PropertyType);
        var list = Activator.CreateInstance(listType);
        var addMethod = listType.GetMethod("Add");

        groupedProperties.ForEach(p =>
        {
            var value = p.GetValue();
            if (value is JsonElement.ArrayEnumerator arrayEnumerator)
            {
                foreach (var item in arrayEnumerator)
                {
                    object? itemValue = item.ValueKind switch
                    {
                        JsonValueKind.String => item.GetString(),
                        JsonValueKind.Number => item.GetInt32(),
                        JsonValueKind.True => true,
                        JsonValueKind.False => false,
                        _ => item.ToString()
                    };
                    if (itemValue != null)
                        addMethod?.Invoke(list, [ChangeType(itemValue, propertyInfo.PropertyType)!]);
                }
            }
            else
            {
                addMethod?.Invoke(list, [ChangeType(value, propertyInfo.PropertyType)!]);
            }
        });

        return Expression.Call(Expression.Constant(list), listType.GetMethod("Contains")!, member);
    }

    private static object? ChangeType(object value, Type propertyType)
    {
        try
        {
            if (value is null)
                return null;

            if (value.GetType() == propertyType)
                return value;

            var nullableType = Nullable.GetUnderlyingType(propertyType);
            if (nullableType != null)
            {
                propertyType = nullableType;
            }

            if (propertyType.IsEnum)
                return Enum.Parse(propertyType, Convert.ToString(value) ?? string.Empty, true);

            if (propertyType == typeof(string))
                return Convert.ToString(value)?.Trim();

            if (propertyType == typeof(Guid))
            {
                var text = Convert.ToString(value);
                return string.IsNullOrEmpty(text) ? Guid.Empty : new Guid(text);
            }

            return Convert.ChangeType(value, propertyType);
        }
        catch
        {
            return Activator.CreateInstance(propertyType);
        }
    }

    private static Expression GenerateExpression(List<Expression> expressions, ExpressionType operatorType, int index = 0)
    {
        if (index == expressions.Count - 1)
            return expressions[index];

        return operatorType == ExpressionType.And
            ? Expression.And(expressions[index], GenerateExpression(expressions, operatorType, index + 1))
            : Expression.Or(expressions[index], GenerateExpression(expressions, operatorType, index + 1));
    }

    private static IQueryable<TEntity> OrderBy<TEntity>(this IQueryable<TEntity> query, IList<AEIG.Domain.PropertySort> propertiesToSort)
    {
        var entityType = typeof(TEntity);
        var firstOrder = true;

        if (propertiesToSort == null || !propertiesToSort.Any(p => p.IsValid()))
            return query;

        foreach (var propertyToSort in propertiesToSort.Where(p => p.IsValid()))
        {
            var propertyInfo = entityType.GetProperty(propertyToSort.Name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (propertyInfo == null)
                continue;

            var arg = Expression.Parameter(entityType, "x");
            var property = Expression.Property(arg, propertyInfo.Name);
            var selector = Expression.Lambda(property, [arg]);

            var methodName = firstOrder
                ? propertyToSort.Ascending ? "OrderBy" : "OrderByDescending"
                : propertyToSort.Ascending ? "ThenBy" : "ThenByDescending";

            firstOrder = false;

            var method = typeof(Queryable).GetMethods()
                .Where(m => m.Name == methodName && m.IsGenericMethodDefinition)
                .Single(m => m.GetParameters().Length == 2);

            var genericMethod = method.MakeGenericMethod(entityType, propertyInfo.PropertyType);
            query = (IQueryable<TEntity>)genericMethod.Invoke(genericMethod, [query, selector])!;
        }

        return query;
    }
}
