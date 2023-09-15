using Core.Divdados.Domain.UserContext.Commands.Inputs;
using Core.Divdados.Domain.UserContext.Commands.Outputs;
using Core.Divdados.Domain.UserContext.Entities;
using Core.Divdados.Domain.UserContext.Repositories;
using Core.Divdados.Shared.Commands;
using Core.Divdados.Shared.Uow;
using Flunt.Validations;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Divdados.Domain.UserContext.Commands.Handlers;

public sealed class CreateObjectiveHandler : Handler<CreateObjectiveCommand, CreateObjectiveCommandResult>
{
    private readonly IObjectiveRepository _objectiveRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUow _uow;
    private readonly CreateObjectiveCommandResult _commandResult;

    public CreateObjectiveHandler(
        IObjectiveRepository objectiveRepository,
        IUserRepository userRepository,
        IUow uow)
    {
        _objectiveRepository = objectiveRepository;
        _userRepository = userRepository;
        _uow = uow;
        _commandResult = new();
    }

    public override Task<CreateObjectiveCommandResult> Handle(CreateObjectiveCommand command, CancellationToken ct)
    {
        if (!command.Validate())
        {
            AddNotifications(command);
            return Incomplete();
        }

        Validate(command.UserId);
        if (Invalid) return Incomplete();

        var objective = new Objective(
            value: command.Value,
            description: command.Description,
            initialDate: DateTime.Today,
            finalDate: command.FinalDate,
            status: "inProgress",
            order: _objectiveRepository.GetObjectives(command.UserId).Count(),
            userId: command.UserId);
        
        AddNotifications(objective);
        if (Invalid) return Incomplete();

        _commandResult.Objective = _objectiveRepository.Add(objective);
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