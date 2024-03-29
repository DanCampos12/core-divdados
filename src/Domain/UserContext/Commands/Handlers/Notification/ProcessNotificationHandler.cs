﻿using Core.Divdados.Domain.UserContext.Commands.Inputs;
using Core.Divdados.Domain.UserContext.Commands.Outputs;
using Core.Divdados.Domain.UserContext.Entities;
using Core.Divdados.Domain.UserContext.Repositories;
using Core.Divdados.Shared.Commands;
using Core.Divdados.Shared.Uow;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Divdados.Domain.UserContext.Commands.Handlers;

public sealed class ProcessNotificationHandler : Handler<ProcessNotificationCommand, ProcessNotificationCommandResult>
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUow _uow;
    private readonly ProcessNotificationCommandResult _commandResult;

    public ProcessNotificationHandler(
        INotificationRepository notificationRepository,
        IUserRepository userRepository,
        IUow uow)
    {
        _notificationRepository = notificationRepository;
        _userRepository = userRepository;
        _uow = uow;
        _commandResult = new();
    }

    public override Task<ProcessNotificationCommandResult> Handle(ProcessNotificationCommand command, CancellationToken ct)
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

        _commandResult.Notifications = _notificationRepository.Process(command.UserId);
        _uow.Commit();

        return Complete(_commandResult.Notifications);
    }
}
