using Core.Divdados.Domain.UserContext.Commands.Inputs;
using Core.Divdados.Domain.UserContext.Commands.Outputs;
using Core.Divdados.Domain.UserContext.Entities;
using Core.Divdados.Domain.UserContext.Repositories;
using Core.Divdados.Shared.Commands;
using Core.Divdados.Shared.Uow;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Divdados.Domain.UserContext.Commands.Handlers;

public sealed class DeleteOperationHandler : Handler<DeleteOperationCommand, DeleteOperationCommandResult>
{
    private readonly IOperationRepository _operationRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUow _uow;
    private readonly DeleteOperationCommandResult _commandResult;

    public DeleteOperationHandler(
        IOperationRepository operationRepository,
        IUserRepository userRepository,
        IUow uow)
    {
        _operationRepository = operationRepository;
        _userRepository = userRepository;
        _uow = uow;
        _commandResult = new();
    }

    public override Task<DeleteOperationCommandResult> Handle(DeleteOperationCommand command, CancellationToken ct)
    {
        if (!command.Validate())
        {
            AddNotifications(command);
            return Incomplete();
        }

        var user = _userRepository.Get(command.UserId);
        if (user is null)
        {
            AddNotification(nameof(User), $"Usuário informado não encontrado ({command.Id})");
            return Incomplete();
        }

        var operation = _operationRepository.GetOperation(command.Id, command.UserId);
        if (operation is null)
        {
            AddNotification(nameof(command.Id), $"Operação não encontrada");
            return Incomplete();
        }

        AddNotifications(operation);
        if (Invalid) return Incomplete();

        _commandResult.Id = _operationRepository.Delete(operation);
        _uow.Commit();

        return Complete(_commandResult.Id);
    }
}
