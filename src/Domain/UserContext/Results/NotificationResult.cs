using Core.Divdados.Domain.UserContext.Entities;
using System;

namespace Core.Divdados.Domain.UserContext.Results;

public class NotificationResult
{
    public NotificationResult() { }

    private NotificationResult(Notification notification)
    {
        Id = notification.Id;
        Date = notification.Date;
        Type = notification.Type;
        Message = notification.Message;
        Read = notification.Read;
        UserId = notification.UserId;
        ExternalId = notification.ExternalId;
    }

    public Guid Id { get; set; }
    public DateTime Date { get; set; }
    public string Type { get; set; }
    public string Message { get; private set; }
    public bool Read { get; private set; }
    public Guid UserId { get; private set; }
    public Guid? ExternalId { get; private set; }

    public static NotificationResult Create(Notification notification) => new(notification);
}