﻿using Core.Divdados.Domain.UserContext.Commands.Inputs;
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

public sealed class RefreshTokenHandler : Handler<RefreshTokenCommand, RefreshTokenCommandResult>
{
    private readonly IAuthRepository _authRepository;
    private readonly IUserRepository _userRepository;
    private readonly AuthService _authService;
    private readonly IUow _uow;
    private readonly RefreshTokenCommandResult _commandResult;

    public RefreshTokenHandler(
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

    public override Task<RefreshTokenCommandResult> Handle(RefreshTokenCommand command, CancellationToken ct)
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

        var isValidToken = _authService.ValidateToken(command.Id, command.IdToken);
        var isLastUserToken = command.IdToken.Equals(user.LastSessionTokenId);
        if (!isValidToken && !isLastUserToken)
        {
            AddNotification(nameof(User), "Não foi possível autenticar o usuário");
            _authRepository.UpdateToken(user, null);
            _uow.Commit();
            return Incomplete();
        }

        var userIdToken = _authService.GenerateToken(user);
        _authRepository.UpdateToken(user, userIdToken);
        _uow.Commit();
        _commandResult.User = UserResult.Create(user);
        _commandResult.IdToken = userIdToken;

        return Complete(new { _commandResult.User, _commandResult.IdToken });
    }
}