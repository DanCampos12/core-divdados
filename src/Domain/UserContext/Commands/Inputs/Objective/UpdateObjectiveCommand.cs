using Core.Divdados.Domain.UserContext.Commands.Outputs;
using Core.Divdados.Shared.Commands;
using Flunt.Validations;
using System;

namespace Core.Divdados.Domain.UserContext.Commands.Inputs;

public class UpdateObjectiveCommand : Command<UpdateObjectiveCommandResult>
{
    public Guid Id { get; set; }
    public decimal Value { get; set; }
    public string Description { get; set; }
    public Guid UserId { get; set; }

    public override bool Validate()
    {
        AddNotifications(new Contract()
            .Requires()
            .IsNotNullOrEmpty(Id.ToString(), nameof(Id), "Id do objetivo é obrigatório")
            .IsNotNullOrEmpty(Description, nameof(Description), "Descrição do objetivo é obrigatório")
            .HasMaxLengthIfNotNullOrEmpty(Description, 50, nameof(Description), "Descrição do objetivo não pode ter mais que 50 caracteres")
            .IsTrue(Value >= 0, nameof(Value), "Valor precisa ser maior que zero")
            .IsNotNullOrEmpty(UserId.ToString(), nameof(UserId), "Id do usuário é obrigatório"));

        return Valid;
    }
}