using Core.Divdados.Domain.UserContext.Commands.Inputs;
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

public sealed class DeleteObjectiveHandler : Handler<DeleteObjectiveCommand, DeleteObjectiveCommandResult>
{
    private readonly IObjectiveRepository _objectiveRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUow _uow;
    private readonly DeleteCategoryCommandResult _commandResult;

    public DeleteObjectiveHandler(
        IObjectiveRepository objectiveRepository,
        IUserRepository userRepository,
        IUow uow)
    {
        _objectiveRepository = objectiveRepository;
        _userRepository = userRepository;
        _uow = uow;
        _commandResult = new();
    }

    public override Task<DeleteObjectiveCommandResult> Handle(DeleteObjectiveCommand command, CancellationToken ct)
    {
        if (!command.Validate())
        {
            AddNotifications(command);
            return Incomplete();
        }

        Validate(command.UserId);
        if (Invalid) return Incomplete();

        var operation = _objectiveRepository.GetObjective(command.Id, command.UserId);
        if (operation is null)
        {
            AddNotification(nameof(command.Id), $"Operação não encontrada");
            return Incomplete();
        }

        AddNotifications(operation);
        if (Invalid) return Incomplete();

        _commandResult.Id = _objectiveRepository.Delete(operation);
        _uow.Commit();

        return Complete(_commandResult.Id);
    }

    private void Validate(Guid userId)
    {
        AddNotifications(new Contract()
            .Requires()
            .IsFalse(_userRepository.Get(userId) is null, nameof(User), $"Usuário inexistente"));
    }
}
