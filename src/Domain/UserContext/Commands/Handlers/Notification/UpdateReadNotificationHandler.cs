using Core.Divdados.Domain.UserContext.Commands.Inputs;
using Core.Divdados.Domain.UserContext.Commands.Outputs;
using Core.Divdados.Domain.UserContext.Entities;
using Core.Divdados.Domain.UserContext.Repositories;
using Core.Divdados.Shared.Commands;
using Core.Divdados.Shared.Uow;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Divdados.Domain.UserContext.Commands.Handlers;

public sealed class UpdateReadNotificationHandler : Handler<UpdateReadNotificationCommand, UpdateReadNotificationCommandResult>
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUow _uow;
    private readonly UpdateReadNotificationCommandResult _commandResult;

    public UpdateReadNotificationHandler(
        INotificationRepository notificationRepository,
        IUserRepository userRepository,
        IUow uow)
    {
        _notificationRepository = notificationRepository;
        _userRepository = userRepository;
        _uow = uow;
        _commandResult = new();
    }

    public override Task<UpdateReadNotificationCommandResult> Handle(UpdateReadNotificationCommand command, CancellationToken ct)
    {
        if (!command.Validate())
        {
            AddNotifications(command);
            return Incomplete();
        }

        var user = _userRepository.Get(command.UserId);
        if (user is null)
        {
            AddNotification(nameof(User), $"Usuário informado não encontrado ({command.UserId})");
            return Incomplete();
        }

        var notificationsToUpdate = new List<Notification>();
        foreach (var notification in command.Notifications)
        {
            var notificationToUpdate = _notificationRepository.Get(notification.Id);
            notificationToUpdate.UpdateRead(true);
            notificationsToUpdate.Add(notificationToUpdate);
        }

        _commandResult.Notifications = _notificationRepository.UpdateRange(notificationsToUpdate);
        _uow.Commit();

        return Complete(_commandResult.Notifications);
    }
}
