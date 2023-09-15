using Core.Divdados.Shared.Entities;
using Flunt.Validations;
using System;

namespace Core.Divdados.Domain.UserContext.Entities;

public sealed class Event : Entity
{
    public decimal Value { get; private set; }
    public char Type { get; private set; }
    public string Description { get; private set; }
    public DateTime InitialDate { get; private set; }
    public DateTime FinalDate { get; private set; }
    public string Period { get; private set; }
    public Guid UserId { get; private set; }
    public Guid CategoryId { get; private set; }

    private Event() { }
    public Event(
        decimal value,
        char type,
        string description,
        DateTime initialDate,
        DateTime finalDate,
        string period,
        Guid userId, 
        Guid categoryId) 
    {
        Value = value;
        Type = type;
        Description = description;
        InitialDate = initialDate;
        FinalDate = finalDate;
        Period = period;
        UserId = userId;
        CategoryId = categoryId;

        AddNotifications(new Contract()
            .Requires()
            .IsTrue(Value >= 0, nameof(Value), "Valor precisa ser maior que zero")
            .IsNotNullOrEmpty(Type.ToString(), nameof(Type), "Tipo do evento é obrigatório")
            .IsNotNullOrEmpty(Description, nameof(Description), "Descrição do evento é obrigatório")
            .HasMaxLengthIfNotNullOrEmpty(Description, 50, nameof(Description), "Descrição do evento não pode ter mais que 50 caracteres")
            .IsNotNullOrEmpty(InitialDate.ToString(), nameof(InitialDate), "Data inicial do evento é obrigatório")
            .IsNotNullOrEmpty(FinalDate.ToString(), nameof(FinalDate), "Data Final do evento é obrigatório")
            .IsTrue(InitialDate <= FinalDate, nameof(FinalDate), "Data inicial deve ser menor ou igual a data final")
            .IsNotNullOrEmpty(Period.ToString(), nameof(Period),  "Período é obrigatório")
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
}