using Core.Divdados.Domain.UserContext.Entities;
using System;

namespace Core.Divdados.Domain.UserContext.Results;

public class NotificationResult
{
    public NotificationResult() { }

    private NotificationResult(Notification notification)
    {
        Id = notification.Id;
        Date = notification.Date.AddHours(-3);
        Type = notification.Type;
        Title = notification.Title;
        Message = notification.Message;
        Read = notification.Read;
        Removed = notification.Removed;
        UserId = notification.UserId;
        ExternalId = notification.ExternalId;
    }

    public Guid Id { get; set; }
    public DateTime Date { get; set; }
    public string Type { get; set; }
    public string Title { get; set; }
    public string Message { get; set; }
    public bool Read { get; set; }
    public bool Removed { get; set; }
    public Guid UserId { get; set; }
    public Guid? ExternalId { get; set; }

    public static NotificationResult Create(Notification notification) => new(notification);
}