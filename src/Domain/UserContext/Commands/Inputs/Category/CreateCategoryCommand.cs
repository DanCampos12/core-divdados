using Core.Divdados.Domain.UserContext.Commands.Outputs;
using Core.Divdados.Shared.Commands;
using Flunt.Validations;
using System;

namespace Core.Divdados.Domain.UserContext.Commands.Inputs;

public class CreateCategoryCommand : Command<CreateCategoryCommandResult>
{
    public string Name { get; set; }
    public string Color { get; set; }
    public Guid UserId { get; set; }

    public override bool Validate()
    {
        AddNotifications(new Contract()
            .Requires()
            .IsNotNullOrEmpty(Name, nameof(Name), "Nome da categoria é obrigatória")
            .HasMaxLengthIfNotNullOrEmpty(Name, 50, nameof(Name), "Nome da categoria não pode ter mais que 50 caracteres")
            .IsNotNullOrEmpty(Color, nameof(Color), "Cor da categoria é obrigatória")
            .HasMaxLengthIfNotNullOrEmpty(Color, 7, nameof(Color), "Cor da categoria não pode ter mais que 7 caracteres")
            .IsNotNullOrEmpty(UserId.ToString(), nameof(UserId), "Id do usuário é obrigatório"));

        return Valid;
    }
}