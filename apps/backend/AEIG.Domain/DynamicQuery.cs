using System.Linq.Expressions;
using System.Text.Json;
using System.Text.Json.Serialization;
using AEIG.Domain.Enums;

namespace AEIG.Domain;

public record DynamicQuery
{
    public int PageNumber { get; set; } = 0;
    public int PageSize { get; set; } = 20;

    public ExpressionType Operator { get; set; } = ExpressionType.And;

    [JsonIgnore]
    public DynamicQueryResultType ResultType { get; set; } = DynamicQueryResultType.Paginated;

    [JsonIgnore]
    public int Skip => PageNumber * PageSize;

    public IList<PropertySort>? OrderBy { get; set; }
    public string? OrderByString { get; set; } = null!;
    public IList<PropertyFilter>? Filter { get; set; }
    public string? FilterString { get; set; } = null!;

    public override string ToString() => JsonSerializer.Serialize(this);
}

public record PropertySort
{
    public string Name { get; set; } = null!;
    public string Order { get; set; } = null!;

    [JsonIgnore]
    public bool Ascending => Order?.Trim().ToLower() != "desc";

    public bool IsValid() => !string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(Order);
}

public record PropertyFilter
{
    public string Name { get; set; } = null!;
    public ExpressionType Condition { get; set; }
    public object Value { get; set; } = null!;

    public object GetValue()
    {
        if (Value is JsonElement jsonElement)
        {
            return jsonElement.ValueKind switch
            {
                JsonValueKind.Array => jsonElement.EnumerateArray(),
                JsonValueKind.String => jsonElement.GetString()!,
                JsonValueKind.Number => jsonElement.GetInt32(),
                JsonValueKind.True => true,
                JsonValueKind.False => false,
                JsonValueKind.Null => null!,
                _ => jsonElement.ToString(),
            };
        }

        return Value;
    }
}
