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

public sealed class ChangePasswordHandler : Handler<ChangePasswordCommand, ChangePasswordCommandResult>
{
    private readonly IAuthRepository _authRepository;
    private readonly IUserRepository _userRepository;
    private readonly AuthService _authService;
    private readonly IUow _uow;
    private readonly ChangePasswordCommandResult _commandResult;

    public ChangePasswordHandler(
        IAuthRepository authRepository,
        IUserRepository userRepository,
        IUow uow,
        IConfiguration configuration)
    {
        _authRepository = authRepository;
        _userRepository = userRepository;
        _authService = new AuthService(configuration.GetSection("Settings").Get<Settings>().JwtBearer);
        _uow = uow;
        _commandResult = new();
    }

    public override Task<ChangePasswordCommandResult> Handle(ChangePasswordCommand command, CancellationToken ct)
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

        if (!AuthService.ValidatePassword(command.Password, user.Password))
        {
            AddNotification(nameof(command.Password), $"Senha informada não corresponde");
            return Incomplete();
        }

        user.UpdatePassword(AuthService.EncryptPassword(command.NewPassword));
        _commandResult.User = _userRepository.Update(user);
        _commandResult.IdToken = _authService.GenerateToken(user);
        _uow.Commit();

        return Complete(new { _commandResult.User, _commandResult.IdToken });
    }
}
