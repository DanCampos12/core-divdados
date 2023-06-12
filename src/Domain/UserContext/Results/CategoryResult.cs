using Core.Divdados.Domain.UserContext.Entities;
using System;

namespace Core.Divdados.Domain.UserContext.Results;

public class CategoryResult
{
    public CategoryResult() { }

    private CategoryResult(Category category)
    {
        Id = category.Id;
        Name = category.Name;
        Color = category.Color;
        UserId = category.UserId;
    }

    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Color { get; set; }
    public Guid UserId { get; set; }

    public static CategoryResult Create(Category category) => new(category);
}
