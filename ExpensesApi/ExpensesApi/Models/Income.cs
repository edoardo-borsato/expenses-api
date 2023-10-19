using System.Text.Json.Serialization;

namespace ExpensesApi.Models;

public record Income
{
    [JsonPropertyName("id")]
    public Guid? Id { get; init; }

    [JsonPropertyName("details")]
    public IncomeDetails? IncomeDetails { get; init; }
}