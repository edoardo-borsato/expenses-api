using System.Text.Json.Serialization;

namespace ExpensesApi.Models;

public record Expense
{
    [JsonPropertyName("id")]
    public Guid? Id { get; init; }

    [JsonPropertyName("details")]
    public ExpenseDetails? ExpenseDetails { get; init; }
}