using Core.Divdados.Domain.UserContext.Commands.Inputs;
using Core.Divdados.Domain.UserContext.Commands.Outputs;
using Core.Divdados.Domain.UserContext.Entities;
using Core.Divdados.Domain.UserContext.Repositories;
using Core.Divdados.Domain.UserContext.Results;
using Core.Divdados.Domain.UserContext.Services;
using Core.Divdados.Shared.Commands;
using Core.Divdados.Shared.Uow;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Divdados.Domain.UserContext.Commands.Handlers;

public sealed class SignInHandler : Handler<SignInCommand, SignInCommandResult>
{
    private readonly IUserRepository _userRepository;
    private readonly AuthService _authService;
    private readonly IUow _uow;
    private readonly SignInCommandResult _commandResult;

    public SignInHandler(
        IUserRepository userRepository,
        IUow uow,
        IConfiguration configuration)
    {
        _userRepository = userRepository;
        _authService = new AuthService(configuration.GetSection("Settings").Get<Settings>().JwtBearer);
        _uow = uow;
        _commandResult = new();
    }

    public override Task<SignInCommandResult> Handle(SignInCommand command, CancellationToken ct)
    {
        if (!command.Validate())
        {
            AddNotifications(command);
            return Incomplete();
        }

        var user = _userRepository.GetByEmail(command.Email);
        if (user is null)
        {
            AddNotification(nameof(User), "Email ou senha não correspondem");
            return Incomplete();
        }

        if (!AuthService.ValidatePassword(command.Password, user.Password))
        {
            AddNotification(nameof(User), "Email ou senha não correspondem");
            return Incomplete();
        }

        var userPreference = _userRepository.GetPreference(user.Id);
        var userIdToken = _authService.GenerateToken(user, DateTime.UtcNow.AddDays(3));
        _commandResult.User = UserResult.Create(user, userPreference);
        _commandResult.IdToken = userIdToken;
        return Complete(new { _commandResult.User, _commandResult.IdToken });
    }
}
