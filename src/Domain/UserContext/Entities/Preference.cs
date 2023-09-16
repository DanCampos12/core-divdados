using Core.Divdados.Shared.Entities;
using Flunt.Validations;
using System;

namespace Core.Divdados.Domain.UserContext.Entities;

public sealed class Preference : Entity
{
    public bool Dark { get; private set; }
    public bool DisplayValues { get; private set; }
    public Guid UserId { get; private set; }

    private Preference() { }
    public Preference(Guid userId) 
    {
        Dark = false;
        DisplayValues = true;
        UserId = userId;

        AddNotifications(new Contract()
        .Requires()
        .IsNotNullOrEmpty(UserId.ToString(), nameof(UserId), "Id do usuário é obrigatório"));
    }

    public void Update(bool dark, bool displayValues)
    {
        Dark = dark;
        DisplayValues = displayValues;
        AddNotifications(this);
    }
}