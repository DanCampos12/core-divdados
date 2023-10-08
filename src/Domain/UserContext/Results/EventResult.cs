using Core.Divdados.Domain.UserContext.Entities;
using System;

namespace Core.Divdados.Domain.UserContext.Results;

public class EventResult
{
    public EventResult() { }

    private EventResult(Event @event, Category category, bool completed)
    {
        Id = @event.Id;
        Value = @event.Value;
        Type = @event.Type;
        Description = @event.Description;
        InitialDate = @event.InitialDate;
        FinalDate = @event.FinalDate;
        Period = @event.Period.Trim();
        UserId = @event.UserId;
        CategoryId = category.Id;
        CategoryName = category.Name;
        Completed = completed;
    }

    public Guid Id { get; set; }
    public decimal Value { get; set; }
    public char Type { get; set; }
    public string Description { get; set; }
    public DateTime InitialDate { get; set; }
    public DateTime FinalDate { get; set; }
    public string Period { get; set; }
    public Guid UserId { get; set; }
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; }
    public bool Completed { get; set; }

    public static EventResult Create(Event @event, Category category, bool completed) => new(@event, category, completed);
}
