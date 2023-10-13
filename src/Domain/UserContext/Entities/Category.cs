using Core.Divdados.Shared.Entities;
using Flunt.Validations;
using System;
using System.Net;

namespace Core.Divdados.Domain.UserContext.Entities;

public sealed class Category : Entity
{
    public string Name { get; private set; }
    public string Color { get; private set; }
    public bool IsAutomaticInput { get; private set; }
    public decimal? MaxValueMonthly { get; private set; }
    public Guid UserId { get; private set; }

    private Category() { }
    public Category(
        string name,
        string color,
        Guid userId, 
        bool isAutomaticInput,
        decimal? maxValueMonthly) 
    {
        Name = name;
        Color = color;
        UserId = userId;
        IsAutomaticInput = isAutomaticInput;
        MaxValueMonthly = maxValueMonthly;

        AddNotifications(new Contract()
            .Requires()
            .IsNotNullOrEmpty(Name, nameof(Name), "Nome da categoria é obrigatória")
            .HasMaxLengthIfNotNullOrEmpty(Name, 50, nameof(Name), "Nome da categoria não pode ter mais que 50 caracteres")
            .IsNotNullOrEmpty(Color, nameof(Color), "Cor da categoria é obrigatória")
            .HasMaxLengthIfNotNullOrEmpty(Color, 7, nameof(Color), "Cor da categoria não pode ter mais que 7 caracteres")
            .IsNotNullOrEmpty(UserId.ToString(), nameof(UserId), "Id do usuário é obrigatório"));
    }

    public void Update(string name, string color)
    {
        Name = name;
        Color = color;
        AddNotifications(this);
    }
}