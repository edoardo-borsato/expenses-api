﻿using System.ComponentModel.DataAnnotations;
using ExpensesApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace ExpensesApi.Controllers;

public record GetAllQueryParameters
{
    [FromQuery(Name = "from")]
    public string? From { get; init; }

    [FromQuery(Name = "to")]
    public string? To { get; init; }

    [FromQuery(Name = "in")]
    public string? In { get; init; }

    [FromQuery(Name = "paymentMethod")]
    public PaymentMethod? PaymentMethod { get; init; }
}