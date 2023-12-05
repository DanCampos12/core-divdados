using Core.Divdados.Domain.UserContext.Commands.Outputs;
using Core.Divdados.Shared.Commands;
using Flunt.Validations;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Divdados.Domain.UserContext.Commands.Inputs;

public record ObjectiveOrder(Guid Id, int Order);

public class ReorderObjectivesCommand : Command<ReorderObjectivesCommandResult>
{
    public Guid UserId { get; set; }
    public IEnumerable<ObjectiveOrder> ObjectivesOrder { get; set; }

    public override bool Validate()
    {
        AddNotifications(new Contract()
            .Requires()
            .IsNotNullOrEmpty(UserId.ToString(), nameof(UserId), "Id do usuário é obrigatório")
            .IsTrue(ObjectivesOrder.Any(), nameof(ObjectivesOrder), "Lista de objetivos é obrigatória"));

        return Valid;
    }
}
