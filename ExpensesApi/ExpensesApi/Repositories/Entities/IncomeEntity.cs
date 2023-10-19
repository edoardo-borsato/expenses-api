using Newtonsoft.Json;

namespace ExpensesApi.Repositories.Entities;

// Apparently [JsonPropertyName("<name>")] does not work, but [JsonProperty(PropertyName = "<name>")] does the job
public record IncomeEntity
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
}