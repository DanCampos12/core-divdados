using Core.Divdados.Shared.Entities;
using Flunt.Validations;
using System;

namespace Core.Divdados.Domain.UserContext.Entities;

public sealed class Notification : Entity
{
    public DateTime Date { get; set; }
    public string Type { get; set; }
    public string Message { get; private set; }
    public bool Read { get; private set; }
    public Guid UserId { get; private set; }

    private Notification() { }
    public Notification(
        string message,
        string type,
        bool read, 
        Guid userId) 
    {
        Date = DateTime.Now;
        Message = message;
        Type = type;
        Read = read;
        UserId = userId;

        AddNotifications(new Contract()
            .Requires()
            .IsNotNullOrEmpty(Message, nameof(Message), "Mensagem da notificação é obrigatória")
            .HasMaxLengthIfNotNullOrEmpty(Message, 100, nameof(Message), "Mensagem da notificação não pode ter mais que 100 caracteres")
            .IsNotNullOrEmpty(UserId.ToString(), nameof(UserId), "Id do usuário é obrigatório"));
    }

    public void Update(bool read)
    {
        Read = read;
        AddNotifications(this);
    }
}