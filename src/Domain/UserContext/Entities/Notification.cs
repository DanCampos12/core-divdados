using Core.Divdados.Shared.Entities;
using Flunt.Validations;
using System;

namespace Core.Divdados.Domain.UserContext.Entities;

public sealed class Notification : Entity
{
    public DateTime Date { get; private set; }
    public string Type { get; private set; }
    public string Title { get; private set; }
    public string Message { get; private set; }
    public bool Read { get; private set; }
    public bool Removed { get; private set; }
    public Guid UserId { get; private set; }
    public Guid? ExternalId { get; private set; }

    private Notification() { }
    public Notification(
        string title,
        string message,
        string type,
        Guid userId, 
        Guid? externalId) 
    {
        Date = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.Local, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));
        Title = title;
        Message = message;
        Type = type;
        Read = false;
        Removed = false;
        UserId = userId;
        ExternalId = externalId;

        AddNotifications(new Contract()
            .Requires()
            .IsNotNullOrEmpty(Title, nameof(Title), "Título da notificação é obrigatória")
            .HasMaxLengthIfNotNullOrEmpty(Title, 50, nameof(Title), "Título da notificação não pode ter mais que 50 caracteres")
            .IsNotNullOrEmpty(Message, nameof(Message), "Mensagem da notificação é obrigatória")
            .HasMaxLengthIfNotNullOrEmpty(Message, 500, nameof(Message), "Mensagem da notificação não pode ter mais que 500 caracteres")
            .IsNotNullOrEmpty(Type, nameof(Type), "Tipo da notificação é obrigatória")
            .IsNotNullOrEmpty(UserId.ToString(), nameof(UserId), "Id do usuário é obrigatório"));
    }

    public void UpdateRead(bool read)
    {
        Read = read;
        AddNotifications(this);
    }

    public void UpdateRemoved(bool removed)
    {
        Removed = removed;
        AddNotifications(this);
    }
}