using Core.Divdados.Domain.UserContext.Commands.Inputs;
using Core.Divdados.Domain.UserContext.Commands.Outputs;
using Core.Divdados.Domain.UserContext.Entities;
using Core.Divdados.Domain.UserContext.Repositories;
using Core.Divdados.Domain.UserContext.Results;
using Core.Divdados.Shared.Commands;
using Core.Divdados.Shared.Uow;
using Flunt.Validations;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Divdados.Domain.UserContext.Commands.Handlers;

public sealed class DeleteUserHandler : Handler<DeleteUserCommand, DeleteUserCommandResult>
{
    private readonly IUserRepository _userRepository;
    private readonly IUow _uow;
    private readonly DeleteUserCommandResult _commandResult;

    public DeleteUserHandler(IUserRepository userRepository, IUow uow)
    {
        _userRepository = userRepository;
        _uow = uow;
        _commandResult = new();
    }

    public override Task<DeleteUserCommandResult> Handle(DeleteUserCommand command, CancellationToken ct)
    {
        if (!command.Validate())
        {
            AddNotifications(command);
            return Incomplete();
        }

        var user = _userRepository.Get(command.Id);
        if (user is null)
        {
            AddNotification(nameof(User), $"Usuário informado não encontrado ({command.Id})");
            return Incomplete();
        }

        if (!user.Password.Equals(command.Password))
        {
            AddNotification(nameof(command.Password), $"Senha informada não corresponde");
            return Incomplete();
        }

        AddNotifications(user);
        if (Invalid) return Incomplete();

        _commandResult.Id = _userRepository.Delete(user);
        _uow.Commit();

        return Complete(_commandResult.Id);
    }
}
