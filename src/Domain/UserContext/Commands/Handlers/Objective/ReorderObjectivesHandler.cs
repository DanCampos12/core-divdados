using Core.Divdados.Domain.UserContext.Commands.Inputs;
using Core.Divdados.Domain.UserContext.Commands.Outputs;
using Core.Divdados.Domain.UserContext.Entities;
using Core.Divdados.Domain.UserContext.Repositories;
using Core.Divdados.Shared.Commands;
using Core.Divdados.Shared.Uow;
using Flunt.Validations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Divdados.Domain.UserContext.Commands.Handlers;

public sealed class ReorderObjectivesHandler : Handler<ReorderObjectivesCommand, ReorderObjectivesCommandResult>
{
    private readonly IObjectiveRepository _objectiveRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUow _uow;
    private readonly ProcessObjectivesCommandResult _commandResult;

    public ReorderObjectivesHandler(
        IObjectiveRepository objectiveRepository,
        IUserRepository userRepository,
        IUow uow)
    {
        _objectiveRepository = objectiveRepository;
        _userRepository = userRepository;
        _uow = uow;
        _commandResult = new();
    }

    public override Task<ReorderObjectivesCommandResult> Handle(ReorderObjectivesCommand command, CancellationToken ct)
    {
        if (!command.Validate())
        {
            AddNotifications(command);
            return Incomplete();
        }

        Validate(command.UserId, command.ObjectivesOrder);
        if (Invalid) return Incomplete();

        _commandResult.Objectives = _objectiveRepository.Reorder(command.UserId, command.ObjectivesOrder);
        _uow.Commit();

        return Complete(_commandResult.Objectives);
    }

    private void Validate(Guid userId, IEnumerable<ObjectiveOrder> objectiveOrders)
    {
        AddNotifications(new Contract()
            .Requires()
            .IsFalse(_userRepository.Get(userId) is null, nameof(User), "Usuário inexistente")
            .IsFalse(objectiveOrders
                .Where(x => _objectiveRepository.GetObjective(x.Id, userId) is null || x.Order < 0).Any(), 
                nameof(Objective),
                "A lista possui objetivos inexistentes ou inválidos"));
    }
}
