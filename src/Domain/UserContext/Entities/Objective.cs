using Core.Divdados.Shared.Entities;
using Flunt.Validations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Core.Divdados.Domain.UserContext.Entities;

public sealed class Objective : Entity
{
    public decimal Value { get; private set; }
    public string Description { get; private set; }
    public DateTime InitialDate { get; private set; }
    public DateTime FinalDate { get; private set; }
    public string Status { get; private set; }
    public int Order { get; private set; }
    public Guid UserId { get; private set; }

    private Objective() { }
    public Objective(
        decimal value,
        string description,
        DateTime initialDate,
        DateTime finalDate,
        string status,
        int order, 
        Guid userId) 
    {
        Value = value;
        Description = description;
        InitialDate = initialDate;
        FinalDate = finalDate;
        Status = status;
        Order = order;
        UserId = userId;

        AddNotifications(new Contract()
            .Requires()
            .IsNotNullOrEmpty(Description, nameof(Description), "Descrição do objetivo é obrigatório")
            .HasMaxLengthIfNotNullOrEmpty(Description, 50, nameof(Description), "Descrição do objetivo não pode ter mais que 50 caracteres")
            .IsTrue(Value >= 0, nameof(Value), "Valor precisa ser maior que zero")
            .IsNotNullOrEmpty(FinalDate.ToString(), nameof(FinalDate), "Date inicial do objetivo é obrigatório")
            .IsNotNullOrEmpty(FinalDate.ToString(), nameof(FinalDate), "Data final do objetivo é obrigatório")
            .IsTrue(FinalDate >= InitialDate, nameof(Value), "Data final deve ser maior ou igual a data inicial")
            .IsNotNullOrEmpty(Status, nameof(Status), "Status do objetivo é obrigatório")
            .IsNotNull(Order, nameof(Status), "Ordenação do objetivo é obrigatório")
            .IsNotNullOrEmpty(UserId.ToString(), nameof(UserId), "Id do usuário é obrigatório"));
    }

    public void Update(decimal value, string description)
    {
        Value = value;
        Description = description;
        AddNotifications(this);
    }
}