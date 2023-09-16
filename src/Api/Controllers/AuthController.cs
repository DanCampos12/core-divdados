using Core.Divdados.Domain.UserContext.Commands.Inputs;
using Core.Divdados.Domain.UserContext.Commands.Outputs;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Core.Divdados.Api.Controllers;

public class AuthController : Controller
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Método que faz a autenticação de um usuário com email e senha
    /// </summary>
    /// <param name="command">Informações do usuário</param>
    /// <returns>Usuário e Token</returns>
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SignInCommandResult))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpPost("v1/users/auth/sign-in")]
    public async Task<IActionResult> SignIn([FromBody] SignInCommand command)
    {
        var commandResult = await _mediator.Send(command);
        return Response(commandResult);
    }

    /// <summary>
    /// Método que atualiza o token de acesso de um usuário
    /// </summary>
    /// <param name="userId">Id do usuário</param>
    /// <param name="command">Informações do usuário</param>
    /// <returns>Usuário e Token</returns>
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RefreshTokenCommandResult))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpPost("v1/users/{userId:guid}/auth/refresh-token")]
    public async Task<IActionResult> RefreshToken(Guid userId, [FromBody] RefreshTokenCommand command)
    {
        command.Id = userId;
        var commandResult = await _mediator.Send(command);

        if (commandResult.Invalid)
            return Unauthorized($"Falha na autenticação do usuário ({command.Id})");

        return Response(commandResult);
    }

    /// <summary>
    /// Método que altera a senha de um usuário
    /// </summary>
    /// <param name="userId">Id do usuário</param>
    /// <param name="command">Informações da nova senha</param>
    /// <returns>Usuário e Token</returns>
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ChangePasswordCommandResult))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpPut("v1/users/{userId:guid}/auth/change-password")]
    public async Task<IActionResult> ChangePassword(Guid userId, [FromBody] ChangePasswordCommand command)
    {
        command.UserId = userId;
        var commandResult = await _mediator.Send(command);
        return Response(commandResult);
    }
}