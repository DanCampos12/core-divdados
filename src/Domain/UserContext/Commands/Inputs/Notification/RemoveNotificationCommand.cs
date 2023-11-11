using Core.Divdados.Domain.UserContext.Commands.Outputs;
using Core.Divdados.Domain.UserContext.Results;
using Core.Divdados.Shared.Commands;
using Flunt.Validations;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Divdados.Domain.UserContext.Commands.Inputs;

public class RemoveNotificationCommand : Command<RemoveNotificationCommandResult>
{
    public Guid UserId { get; set; }
    public new IEnumerable<NotificationResult> Notifications { get; set; }

    public override bool Validate()
    {
        AddNotifications(new Contract()
            .Requires()
            .IsTrue(Notifications.Any(), nameof(Notifications), "Lista de notificações é obrigatória")
            .IsNotNullOrEmpty(UserId.ToString(), nameof(UserId), "Id do usuário é obrigatório"));

        return Valid;
    }
}