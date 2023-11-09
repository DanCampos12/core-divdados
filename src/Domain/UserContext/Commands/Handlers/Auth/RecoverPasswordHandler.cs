using Core.Divdados.Domain.UserContext.Commands.Inputs;
using Core.Divdados.Domain.UserContext.Commands.Outputs;
using Core.Divdados.Domain.UserContext.Entities;
using Core.Divdados.Domain.UserContext.Repositories;
using Core.Divdados.Domain.UserContext.Services;
using Core.Divdados.Shared.Commands;
using Core.Divdados.Shared.Uow;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Divdados.Domain.UserContext.Commands.Handlers;

public sealed class RecoverPasswordHandler : Handler<RecoverPasswordCommand, RecoverPasswordCommandResult>
{
    private readonly IUserRepository _userRepository;
    private readonly AuthService _authService;
    private readonly AwsService _awsService;
    private readonly IUow _uow;
    private readonly RecoverPasswordCommandResult _commandResult;

    public RecoverPasswordHandler(
        IUserRepository userRepository,
        IUow uow,
        IConfiguration configuration)
    {
        _userRepository = userRepository;
        _authService = new AuthService(configuration.GetSection("Settings").Get<Settings>().JwtBearer);
        _awsService = new AwsService(configuration.GetSection("Settings").Get<Settings>().AWS);
        _uow = uow;
        _commandResult = new();
    }

    public override async Task<RecoverPasswordCommandResult> Handle(RecoverPasswordCommand command, CancellationToken ct)
    {
        if (!command.Validate())
        {
            AddNotifications(command);
            return await Incomplete();
        }

        var user = _userRepository.GetByEmail(command.Email);
        if (user is null)
        {
            AddNotification(nameof(User), $"Usuário informado não encontrado ({command.Email})");
            return await Incomplete();
        }

        var idToken = _authService.GenerateToken(user, DateTime.UtcNow.AddMinutes(30));
        _commandResult.Message = await _userRepository.RecoverPassword(user, idToken, _awsService);
        user.UpdateFlowComplete(false);
        _userRepository.Update(user);
        _uow.Commit();

        return await Complete(new { _commandResult.Message });
    }
}
