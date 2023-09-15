using Core.Divdados.Domain.UserContext.Commands.Outputs;
using Core.Divdados.Domain.UserContext.Entities;
using Core.Divdados.Shared.Commands;
using Flunt.Validations;
using System;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Core.Divdados.Domain.UserContext.Commands.Inputs;

public class UpdateOperationCommand : Command<UpdateOperationCommandResult>
{
    public Guid Id { get; set; }
    public decimal Value { get; set; }
    public string Description { get; set; }
    public Guid CategoryId { get; set; }
    public Guid UserId { get; set; }

    public override bool Validate()
    {
        AddNotifications(new Contract()
            .Requires()
            .IsNotNullOrEmpty(Id.ToString(), nameof(Id), "Id da operação é obrigatório")
            .IsTrue(Value >= 0, nameof(Value), "Valor precisa ser maior que zero")
            .IsNotNullOrEmpty(Description, nameof(Description), "Descrição da operação é obrigatória")
            .HasMaxLengthIfNotNullOrEmpty(Description, 50, nameof(Description), "Descrição da operação não pode ter mais que 50 caracteres")
            .IsNotNullOrEmpty(CategoryId.ToString(), nameof(CategoryId), "Categoria da operação é obrigatório")
            .IsNotNullOrEmpty(UserId.ToString(), nameof(UserId), "Id do usuário é obrigatório"));

        return Valid;
    }
}