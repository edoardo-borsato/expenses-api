﻿using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ExpensesApi.Models;

public record ExpenseDetails
{
    [Required]
    [JsonPropertyName("value")]
    public double Value { get; init; }

    [JsonPropertyName("date")]
    public DateTimeOffset? Date { get; init; }

    [Required]
    [JsonPropertyName("reason")]
    public string? Reason { get; init; }

    [JsonPropertyName("category")]
    public Category? Category { get; init; }
}