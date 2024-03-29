﻿using Core.Divdados.Domain.UserContext.Commands.Inputs;
using Core.Divdados.Domain.UserContext.Commands.Outputs;
using Core.Divdados.Domain.UserContext.Entities;
using Core.Divdados.Domain.UserContext.Repositories;
using Core.Divdados.Shared.Commands;
using Core.Divdados.Shared.Uow;
using Flunt.Validations;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Divdados.Domain.UserContext.Commands.Handlers;

public sealed class UpdateObjectiveHandler : Handler<UpdateObjectiveCommand, UpdateObjectiveCommandResult>
{
    private readonly IObjectiveRepository _objectiveRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUow _uow;
    private readonly UpdateObjectiveCommandResult _commandResult;

    public UpdateObjectiveHandler(
        IObjectiveRepository objectiveRepository,
        IUserRepository userRepository,
        IUow uow)
    {
        _objectiveRepository = objectiveRepository;
        _userRepository = userRepository;
        _uow = uow;
        _commandResult = new();
    }

    public override Task<UpdateObjectiveCommandResult> Handle(UpdateObjectiveCommand command, CancellationToken ct)
    {
        if (!command.Validate())
        {
            AddNotifications(command);
            return Incomplete();
        }

        Validate(command.UserId);
        if (Invalid) return Incomplete();

        var objective = _objectiveRepository.GetObjective(command.Id, command.UserId);
        if (objective is null)
        {
            AddNotification(nameof(command.Id), $"Objetivo não encontrado");
            return Incomplete();
        }

        objective.Update(command.Value, command.Description);
        AddNotifications(objective);
        if (Invalid) return Incomplete();

        _commandResult.Objective = _objectiveRepository.Update(objective);
        _uow.Commit();

        return Complete(_commandResult.Objective);
    }

    private void Validate(Guid userId)
    {
        AddNotifications(new Contract()
            .Requires()
            .IsFalse(_userRepository.Get(userId) is null, nameof(User), $"Usuário inexistente"));
    }
}