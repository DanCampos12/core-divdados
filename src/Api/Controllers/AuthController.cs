using Core.Divdados.Api.Authorizations;
using Core.Divdados.Domain.UserContext.Commands.Inputs;
using Core.Divdados.Domain.UserContext.Results;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
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
    /// <param name="email">Email do usuário</param>
    /// <param name="password">Senha do usuário</param>
    /// <returns>Usuário e Token</returns>
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserResult))]
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
    /// <param name="id">Id do usuário</param>
    /// <param name="command">Informações do usuário</param>
    /// <returns>Usuário e Token</returns>
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserResult))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpPost("v1/users/{id:guid}/auth/refresh-token")]
    [ServiceFilter(typeof(AuthorizationAttribute))]
    public async Task<IActionResult> RefreshToken(Guid id, [FromBody] RefreshTokenCommand command)
    {
        command.Id = id;
        var commandResult = await _mediator.Send(command);

        if (commandResult.Invalid)
            return Unauthorized($"Falha na autenticação do usuário ({command.Id})");

        return Response(commandResult);
    }

    /// <summary>
    /// Método que invalida as autenticações do usuário
    /// </summary>
    /// <param name="id">Id do usuário</param>
    /// <returns>Id do usuário</returns>
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Guid))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpPost("v1/users/{id:guid}/auth/sign-out")]
    [ServiceFilter(typeof(AuthorizationAttribute))]
    public async Task<IActionResult> SignOut(Guid id)
    {
        var command = new SignOutCommand() { Id = id };
        var commandResult = await _mediator.Send(command);
        return Response(commandResult);
    }
}