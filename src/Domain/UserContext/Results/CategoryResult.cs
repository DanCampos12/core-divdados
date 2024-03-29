﻿using Core.Divdados.Domain.UserContext.Entities;
using System;

namespace Core.Divdados.Domain.UserContext.Results;

public class CategoryResult
{
    public CategoryResult() { }

    private CategoryResult(Category category, decimal allocation)
    {
        Id = category.Id;
        Name = category.Name;
        Color = category.Color;
        UserId = category.UserId;
        IsAutomaticInput = category.IsAutomaticInput;
        MaxValueMonthly = category.MaxValueMonthly;
        Allocation = allocation;
    }

    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Color { get; set; }
    public Guid UserId { get; set; }
    public bool IsAutomaticInput { get; set; }  
    public decimal? MaxValueMonthly { get; set; }
    public decimal Allocation { get; set; }

    public static CategoryResult Create(Category category, decimal allocation) => new(category, allocation);
}
