using Core.Divdados.Domain.UserContext.Results;
using Core.Divdados.Shared.Commands;
using System.Collections.Generic;

namespace Core.Divdados.Domain.UserContext.Commands.Outputs;

public class RemoveNotificationCommandResult : CommandResult
{
    public IEnumerable<NotificationResult> Notifications { get; set; }
}