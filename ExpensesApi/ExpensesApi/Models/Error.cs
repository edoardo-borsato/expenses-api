using System.Text.Json.Serialization;

namespace ExpensesApi.Models;

public record Error
{
    [JsonPropertyName("error")]
    public string ErrorMessage { get; set; }
}