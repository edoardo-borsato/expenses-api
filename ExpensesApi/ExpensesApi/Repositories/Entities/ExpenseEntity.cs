using System.Text.Json.Serialization;

namespace ExpensesApi.Repositories.Entities;

public record ExpenseEntity
{
    [JsonPropertyName("email")]
    public string? Id { get; init; }

    [JsonPropertyName("value")]
    public double Value { get; set; }

    [JsonPropertyName("date")]
    public string? Date { get; set; }

    [JsonPropertyName("reason")]
    public string? Reason { get; set; }

    [JsonPropertyName("category")]
    public Category Category { get; set; }
}

public enum Category
{
    Others = 0,
    HousingAndSupplies = 1,
    HealthAndPersonalCare = 2,
    Sport = 3,
    Transportation = 4,
    Clothing = 5,
    Entertainment = 6,
    BillsAndUtilities = 7,
    Pets = 8,
    Insurance = 9,
    Gifts = 10
}