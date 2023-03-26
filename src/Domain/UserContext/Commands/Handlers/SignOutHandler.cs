using Core.Divdados.Domain.UserContext.Commands.Inputs;
using Core.Divdados.Domain.UserContext.Commands.Outputs;
using Core.Divdados.Domain.UserContext.Entities;
using Core.Divdados.Domain.UserContext.Repositories;
using Core.Divdados.Domain.UserContext.Results;
using Core.Divdados.Domain.UserContext.Services;
using Core.Divdados.Shared.Commands;
using Core.Divdados.Shared.Uow;
using Microsoft.Extensions.Configuration;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Divdados.Domain.UserContext.Commands.Handlers;

public sealed class SignOutHandler : Handler<SignOutCommand, SignOutCommandResult>
{
    private readonly IAuthRepository _authRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUow _uow;
    private readonly SignOutCommandResult _commandResult;

    public SignOutHandler(
        IAuthRepository authRepository,
        IUserRepository userRepository,
        IUow uow)
    {
        _authRepository = authRepository;
        _userRepository = userRepository;
        _uow = uow;
        _commandResult = new();
    }

    public override Task<SignOutCommandResult> Handle(SignOutCommand command, CancellationToken ct)
    {
        if (!command.Validate())
        {
            AddNotifications(command);
            return Incomplete();
        }

        var user = _userRepository.Get(command.Id);
        if (user is null)
        {
            AddNotification(nameof(User), $"Usuário ({command.Id}) não encontrado");
            return Incomplete();
        }

        _authRepository.UpdateToken(user, null);
        _uow.Commit();
        _commandResult.Id = user.Id;
        return Complete(_commandResult.Id);
    }
}
