using Core.Divdados.Domain.UserContext.Entities;
using System;

namespace Core.Divdados.Domain.UserContext.Results;

public class ObjectiveResult
{
    public ObjectiveResult() { }

    private ObjectiveResult(Objective objective, decimal progress)
    {
        Id = objective.Id;
        Value = objective.Value;
        Description = objective.Description;
        InitialDate = objective.InitialDate;
        FinalDate = objective.FinalDate;
        Status = objective.Status;
        Order = objective.Order;
        UserId = objective.UserId;
        Progress = progress;
    }

    public Guid Id { get; set; }
    public decimal Value { get; set; }
    public string Description { get; set; }
    public DateTime InitialDate { get; set; }
    public DateTime FinalDate { get; set; }
    public string Status { get; set; }
    public int Order { get; set; }
    public Guid UserId { get; set; }
    public decimal Progress { get; set; }

    public static ObjectiveResult Create(Objective objective, decimal progress) => new(objective, progress);
}
