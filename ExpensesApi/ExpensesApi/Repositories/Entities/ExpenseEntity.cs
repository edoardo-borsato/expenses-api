using Newtonsoft.Json;

namespace ExpensesApi.Repositories.Entities;

// Apparently [JsonPropertyName("<name>")] does not work, but [JsonProperty(PropertyName = "<name>")] does the job
public record ExpenseEntity
{
    // The id property is mandatory
    [JsonProperty("id")]
    public string? Id { get; init; }

    [JsonProperty("username")]
    public string? Username { get; init; }

    [JsonProperty("guid")]
    public string? Guid { get; init; }

    [JsonProperty("value")]
    public double Value { get; set; }

    [JsonProperty("date")]
    public string? Date { get; set; }

    [JsonProperty("reason")]
    public string? Reason { get; set; }

    [JsonProperty("category")]
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