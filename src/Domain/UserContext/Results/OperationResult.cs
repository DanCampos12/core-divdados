using Core.Divdados.Domain.UserContext.Entities;
using System;

namespace Core.Divdados.Domain.UserContext.Results;

public class OperationResult
{
    public OperationResult() { }

    private OperationResult(Operation operation, Category category)
    {
        Id = operation.Id;
        Value = operation.Value;
        Type = operation.Type;
        Description = operation.Description;
        Date = operation.Date;
        Effected = operation.Effected;
        UserId = operation.UserId;
        CategoryId = category.Id;
        CategoryName = category.Name;
        EventId = operation.EventId;
    }

    public Guid Id { get; set; }
    public decimal Value { get; set; }
    public char Type { get; set; }
    public string Description { get; set; }
    public DateTime Date { get; set; }
    public bool Effected { get; set; }
    public Guid UserId { get; set; }
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; }
    public Guid? EventId { get; set; }

    public static OperationResult Create(Operation operation, Category category) => new(operation, category);
}
