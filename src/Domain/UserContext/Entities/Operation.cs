using Core.Divdados.Shared.Entities;
using Flunt.Validations;
using System;

namespace Core.Divdados.Domain.UserContext.Entities;

public sealed class Operation : Entity
{
    public decimal Value { get; private set; }
    public char Type { get; private set; }
    public string Description { get; private set; }
    public DateTime Date { get; private set; }
    public bool Effected { get; private set; }
    public Guid UserId { get; private set; }
    public Guid CategoryId { get; private set; }
    public Guid? EventId { get; private set; }

    private Operation() { }
    public Operation(
        decimal value,
        char type,
        string description,
        DateTime date,
        bool effected,
        Guid userId, 
        Guid categoryId,
        Guid? eventId) 
    {
        Value = value;
        Type = type;
        Description = description;
        Date = date;
        Effected = effected;
        UserId = userId;
        CategoryId = categoryId;
        EventId = eventId;

        AddNotifications(new Contract()
            .Requires()
            .IsTrue(Value >= 0, nameof(Value), "Valor precisa ser maior que zero")
            .IsNotNullOrEmpty(Type.ToString(), nameof(Type), "Tipo da operação é obrigatória")
            .IsNotNullOrEmpty(Description, nameof(Description), "Descrição da operação é obrigatória")
            .HasMaxLengthIfNotNullOrEmpty(Description, 50, nameof(Description), "Descrição da operação não pode ter mais que 50 caracteres")
            .IsNotNullOrEmpty(Date.ToString(), nameof(Date), "Data da operação é obrigatória")
            .IsNotNullOrEmpty(UserId.ToString(), nameof(UserId), "Id do usuário é obrigatório")
            .IsNotNullOrEmpty(CategoryId.ToString(), nameof(CategoryId), "Categoria da operação é obrigatório"));
    }

    public void Update(decimal value, string description, Guid categoryId)
    {
        Value = value;
        Description = description;
        CategoryId = categoryId; 
        AddNotifications(this);
    }

    public void SetEffected (bool effected)
    {
        Effected = effected;
        AddNotifications(this);
    }
}